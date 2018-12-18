using System;
using System.Collections.Generic;
using System.Linq;

namespace DiscordBotGuardian
{
    /// <summary>
    /// This class will be used for selecting Angela Lansbury gifs without poluting the main code
    /// </summary>
    class Angela
    {
        /// <summary>
        /// Used for randomly picking a gif from a list to return as a string
        /// </summary>
        public static string RandomAngela()
        {
            // Generate the list
            List<string> gifs = new List<string>
            {
                "https://media.giphy.com/media/kioZpjoUyj9EA/giphy.gif",
                "https://media.giphy.com/media/l0ExeaMkkgARgOZOw/giphy.gif",
                "https://media.giphy.com/media/26gs9BNQFdzNZZ1ZK/giphy.gif",
                "https://media.giphy.com/media/rETWWyKTPqN9e/giphy.gif",
                "https://media.giphy.com/media/IoDsEKP0fi7bq/giphy.gif",
                "https://media.giphy.com/media/ZikyVyLF7aEaQ/giphy.gif",
                "https://media.giphy.com/media/jYWGpuPdfmllS/giphy.gif",
                "https://media.giphy.com/media/PusmzMI9dyxoc/giphy.gif",
                "https://media.giphy.com/media/JGLYZcomPzv4Q/giphy.gif",
                "https://media.giphy.com/media/cqKCIUQw7Apyg/giphy.gif",
                "https://media.giphy.com/media/SzzDk8omnD9Xq/giphy.gif",
                "https://media.giphy.com/media/l0ExghDSRxU2g55sc/giphy.gif",
                "https://media.giphy.com/media/t3dLl0TGHCxTG/giphy.gif",
                "https://media.giphy.com/media/gNCoR18QIJQIM/giphy.gif",
                "https://media.giphy.com/media/JKAdHLlhTAYh2/giphy.gif",
                "https://media.giphy.com/media/JGLYZcomPzv4Q/giphy.gif",
                "https://media.giphy.com/media/11bi3s0GHQsjSw/giphy.gif",
                "https://media.giphy.com/media/hj8oC5iEHNaI8/giphy.gif",
                "https://media.giphy.com/media/iXNi87RUf36SY/giphy.gif"
            };
            // Return a random gif
            return gifs.PickRandom();

        }
    }
    /// <summary>
    /// Adding on to the list class using linq
    /// </summary>
    public static class EnumerableExtension
    {
        /// <summary>
        /// Tack on pick random to a list
        /// </summary>
        public static T PickRandom<T>(this IEnumerable<T> source)
        {
            // Pick a single random
            return source.PickRandom(1).Single();
        }

        /// <summary>
        /// Take a choice using shuffle
        /// </summary>
        public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count)
        {
            return source.Shuffle().Take(count);
        }

        /// <summary>
        /// Shuffle the list
        /// </summary>
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
        {
            return source.OrderBy(x => Guid.NewGuid());
        }
    }
}
