﻿namespace Model.Users
{
    public struct UserProfile
    {
        public string FirstName { get; set; } //todo limit 100

        public string LastName { get; set; } //todo limit 100

        public string Email { get; set; } //todo regExp 100
        
        public string City { get; set; } //todo limit 256

        public string Country { get; set; } //todo limit 256

        public string PostCode { get; set; } //todo limit 256

        public string Phone { get; set; } //todo regExp
        
        public string Skype { get; set; } //todo limit 6 to 32
        
        public string Telegram { get; set; } //todo regExp
        
        public string About { get; set; } //todo limit 400 chars
               
    }
}