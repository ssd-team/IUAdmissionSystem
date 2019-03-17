﻿using System;
using System.Collections.Generic;
using System.Linq;
using DataModel.Support;
using DataModel.Users;

namespace DataModel.Authentication
{
    public class AuthManager:Singletone<AuthManager>
    {
        private readonly Dictionary<AuthData, AbstractUser> _usersAuthData;

        private readonly TokensRegister _register;
        
        /// <summary>
        /// Get AbstractUser by authToken
        /// </summary>
        /// <param name="authToken">authenticated token</param>
        public AbstractUser this[TokenData authToken] => _register.ValidateAuthToken(authToken);
            
        public AuthManager()
        {
            _register = new TokensRegister();
            _usersAuthData = new Dictionary<AuthData, AbstractUser>();
        }


        /// <summary>
        /// Authenticate user to the system
        /// After auth, user gets unique auth token, with which only one can access to the system
        /// After session user must logout
        /// </summary>
        /// <param name="login">login of the user (email)</param>
        /// <param name="password">password of the user (not encrypted)</param>
        /// <exception cref="ArgumentException">Check on null arguments</exception>
        /// <exception cref="AuthExceptions.UserDoesNotExists">If user's login doesnt' exists in DB</exception>
        /// <exception cref="AuthExceptions.IncorrectPassword">If password incorrect</exception>
        /// <returns>auth token for the user</returns>
        public static TokenData AuthUser(AuthData authData)
        {
            if (authData == null) 
                throw new ArgumentException("Passed parameter was null",nameof(authData));
            
            var user = Instance.DoesUserExists(authData);
            
            return Instance._register.GetAuthToken(user);
        }


        /// <summary>
        /// Register new user with login and password
        /// Also authenticate user
        /// </summary>
        /// <param name="login"> Login of new user</param>
        /// <param name="password">Password for new user</param>
        /// <exception cref="AuthExceptions.RegistrationException">If something went wrong</exception>
        /// <returns>Authenticated Token</returns>
        public static TokenData RegisterUser(AuthData authData, RootEnum[] roots)
        {
            AbstractUser user = new TestUser(roots); //todo factory of creating users 
            Instance._usersAuthData.Add(authData, user);
            try
            {
                return AuthUser(authData);
            }
            catch (AuthExceptions e)
            {
                throw new AuthExceptions.RegistrationException(authData);
            }
        }
        
        
        /// <summary>
        /// Logout user by deletion authtoekn from register
        /// Helps to make sure, that no body use the auth token
        /// Needs for security purposes
        /// </summary>
        /// <param name="authToken">auth token, which was recieved by registration or authentication</param>
        /// <returns>notihng if success, otherwise errors</returns>
        public static void LogOutUser(TokenData authToken)
        {
            Instance._register.FreeToken(authToken);
        }
     
        
        /// <summary>
        /// Auth tokens has TTL, and after validating TTL updates, 
        /// If register doesn't contain authToken, then you must Auth again 
        /// </summary>
        /// <param name="authtoken">authenticate token, which was given with authentication</param>
        /// <exception cref="TokenExceptions.TokenDoesNotExists">If registed doesn't have such token</exception>
        /// <exception cref="TokenExceptions.TokenExpired">If token in register, but expired</exception>
        /// <returns>true if authToken valid, false if not</returns>
        public static bool ValidateAuthToken(TokenData authtoken)
        {
            return Instance._register.ValidateAuthToken(authtoken) != null;
        }

        /// <summary>
        /// Check on user existance
        /// </summary>
        /// <param name="authData">authentication data of some user</param>
        /// <returns>User with such auth data</returns>
        /// <exception cref="AuthExceptions.UserDoesNotExists">If user's login doesnt' exists in DB</exception>
        /// <exception cref="AuthExceptions.IncorrectPassword">If password incorrect</exception>
        private AbstractUser DoesUserExists(AuthData authData)
        {
            var pair = _usersAuthData.FirstOrDefault(n => n.Key.Login.Equals(authData.Login)); 
            
            if (pair.Key == null) 
                throw new AuthExceptions.UserDoesNotExists(authData);
            if (!pair.Key.Password.Equals(authData.Password)) 
                throw  new AuthExceptions.IncorrectPassword(authData);
            
            return pair.Value;
        }    
    }
}