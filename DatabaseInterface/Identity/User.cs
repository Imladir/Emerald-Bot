﻿using EmeraldBot.Model.Characters;
using EmeraldBot.Model.Rolls;
using EmeraldBot.Model.Servers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Identity
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [PersonalData]
        public int ID { get; set; }
        [ProtectedPersonalData]
        public string UserName { get; set; } = "";
        [Required]
        public long DiscordID { get; set; } = 0;
        public string NormalizedUserName { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public bool Verbose { get; set; } = false;
        public DateTime LastUpdate { get; set; } = DateTime.UtcNow;
        public int AccessFailedCount { get; set; } = 0;
        public virtual ICollection<PC> Characters { get; set; }
        public virtual ICollection<PrivateChannel> PrivateChannels { get; set; }
        public virtual ICollection<DefaultCharacter> DefaultCharacters { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<UserRole> Roles { get; set; }
        public virtual ICollection<UserToken> Tokens { get; set; }
        public virtual ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();

        public User()
        {
            GenerateToken();
        }

        public void LoadDefaultCharacters(EmeraldBotContext ctx)
        {
            ctx.Entry(this).Collection(x => DefaultCharacters).Load();
            foreach (var dc in DefaultCharacters) ctx.Entry(dc).Reference(x => x.Server).Load();
        }

        public void LoadPrivateChannels(EmeraldBotContext ctx)
        {
            ctx.Entry(this).Collection(x => PrivateChannels).Query()
                .Include(x => x.Server)
                .Include(x => x.Player).Load();
        }

        public PC GetDefaultCharacter(Server server)
        {
            return DefaultCharacters.SingleOrDefault(x => x.Server == server).Character as PC;
        }

        public string GenerateToken(string password = "")
        {
            if (password == "")
            {
                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var stringChars = new char[12];
                var rng = new RNGCryptoServiceProvider();

                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[RollDice(rng, (byte)chars.Length)];
                }
                password = new string(stringChars);
            }

            var passwordHasher = new PasswordHasher<User>();
            PasswordHash = passwordHasher.HashPassword(this, password);

            return password;
        }

        // From: https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rngcryptoserviceprovider?view=netframework-4.8
        private byte RollDice(RNGCryptoServiceProvider rng, byte numberSides)
        {
            if (numberSides <= 0)
                throw new ArgumentOutOfRangeException("numberSides");

            // Create a byte array to hold the random value.
            byte[] randomNumber = new byte[1];
            do
            {
                // Fill the array with a random value.
                rng.GetBytes(randomNumber);
            }
            while (!IsFairRoll(randomNumber[0], numberSides));
            // Return the random number mod the number
            // of sides.
            return (byte)(randomNumber[0] % numberSides);
        }

        private bool IsFairRoll(byte roll, byte numSides)
        {
            // There are MaxValue / numSides full sets of numbers that can come up
            // in a single byte.  For instance, if we have a 6 sided die, there are
            // 42 full sets of 1-6 that come up.  The 43rd set is incomplete.
            int fullSetsOfValues = byte.MaxValue / numSides;

            // If the roll is within this range of fair values, then we let it continue.
            // In the 6 sided die case, a roll between 0 and 251 is allowed.  (We use
            // < rather than <= since the = portion allows through an extra 0 value).
            // 252 through 255 would provide an extra 0, 1, 2, 3 so they are not fair
            // to use.
            return roll < numSides * fullSetsOfValues;
        }

    }
}
