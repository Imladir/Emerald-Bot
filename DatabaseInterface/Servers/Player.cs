using EmeraldBot.Model.Characters;
using EmeraldBot.Model.Rolls;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EmeraldBot.Model.Servers
{
    public class Player
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required]
        public long DiscordID { get; set; }
        [MaxLength(64)]
        public string Name { get; set; }
        [MaxLength(128)]
        public string Password { get; set; }
        [MaxLength(128)]
        public string Salt { get; set; }
        public bool Verbose { get; set; }
        public DateTime LastUpdated { get; set; }

        public virtual ICollection<GM> IsGMOn { get; set; }
        public virtual ICollection<PC> Characters { get; set; }
        public virtual ICollection<PrivateChannel> PrivateChannels { get; set; }
        public virtual ICollection<DefaultCharacter> DefaultCharacters { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        //public virtual ICollection<Roll> Rolls { get; set; }

        public Player()
        {
            Name = "";
            Verbose = false;
            GenerateToken();
        }

        public bool IsGM(ulong serverID)
        {
            return IsGMOn.Any(x => x.Server.DiscordID == (long)serverID);
        }

        public override bool Equals(object obj)
        {
            return obj is Player player &&
                   (ID == player.ID
                   || DiscordID == player.DiscordID);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(DiscordID);
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
                password = new String(stringChars);
            }

            var salt = Pbkdf2Hasher.GenerateRandomSalt();
            Salt = Convert.ToBase64String(salt);
            Password = Pbkdf2Hasher.ComputeHash(password, salt);
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
            return (byte)((randomNumber[0] % numberSides));
        }

        private bool IsFairRoll(byte roll, byte numSides)
        {
            // There are MaxValue / numSides full sets of numbers that can come up
            // in a single byte.  For instance, if we have a 6 sided die, there are
            // 42 full sets of 1-6 that come up.  The 43rd set is incomplete.
            int fullSetsOfValues = Byte.MaxValue / numSides;

            // If the roll is within this range of fair values, then we let it continue.
            // In the 6 sided die case, a roll between 0 and 251 is allowed.  (We use
            // < rather than <= since the = portion allows through an extra 0 value).
            // 252 through 255 would provide an extra 0, 1, 2, 3 so they are not fair
            // to use.
            return roll < numSides * fullSetsOfValues;
        }

    }
}
