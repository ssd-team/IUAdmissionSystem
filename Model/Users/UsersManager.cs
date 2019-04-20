﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Model.Support;

namespace Model.Users
{
    class UsersManager:Singletone<UsersManager>
    {

        private Dictionary<Type, List<AbstractUser>> _specificUserLists;


        public UsersManager()
        {

            _specificUserLists = new Dictionary<Type, List<AbstractUser>>();

            var subclassTypes = Assembly.GetAssembly(typeof(AbstractUser)).GetTypes().Where(t => t.IsSubclassOf(typeof(AbstractUser)) && !t.IsAbstract);
            foreach (var subclassType in subclassTypes)
            {

                Type genericListType = typeof(List<>).MakeGenericType(subclassType);
                Activator.CreateInstance(genericListType);
                _specificUserLists.Add(subclassType, (List<AbstractUser>) Activator.CreateInstance(genericListType));
            }
        }

        public static AbstractUser CreateUser(RootEnum[] roots)
        {
            //todo logic of understanding roots
            CandidateUser user = new CandidateUser();
            Instance._specificUserLists[user.GetType()].Add(user);
            return user;
        }

        public static AbstractUser GetUserByID(int id) 
            => Instance._specificUserLists.Values.SelectMany(n => n).Single(i => i.id == id);


        public static T GetUserByID<T>(int id) where T : AbstractUser 
            => Instance._specificUserLists[typeof(T)].SingleOrDefault(i => i.id == id) as T;



        public static void DeleteUserById(int id)
        {
            foreach (var list in Instance._specificUserLists.Values)
            {
                if(list.RemoveAll(n=> n.id == id) > 0)
                    break;
            }
        }
    }
}