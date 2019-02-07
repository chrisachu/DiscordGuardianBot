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
                "https://i.imgur.com/Zw9N0Zj.gifv",
                "https://i.imgur.com/HNHI7R8.gifv",
                "https://i.imgur.com/g4AsNST.gifv",
                "https://i.imgur.com/IUPakM0.gifv",
                "https://i.imgur.com/L8og9FQ.gifv",
                "https://i.imgur.com/XrWZHfw.gifv",
                "https://i.imgur.com/1YP3k5H.gifv",
                "https://i.imgur.com/pguPnex.gifv",
                "https://i.imgur.com/llM9VLc.gifv",
                "https://i.imgur.com/LxUkPqc.gifv",
                "https://i.imgur.com/lcvipK6.gifv",
                "https://i.imgur.com/1pT1his.gifv",
                "https://i.imgur.com/qXOyevq.gifv",
                "https://i.imgur.com/QRLm4Km.gifv",
                "https://i.imgur.com/e7cxdOR.gifv",
                "https://i.imgur.com/darsAux.gifv",
                "https://i.imgur.com/YmslKNT.gifv",
                "https://i.imgur.com/R74Wt1u.gifv",
                "https://i.imgur.com/iOsRfOp.gifv",
                "https://i.imgur.com/QQRrH4m.gifv",
                "https://i.imgur.com/pXV6JTO.gifv",
                "https://i.imgur.com/denNrt8.gifv",
                "https://i.imgur.com/Du2vvXx.gifv",
                "https://i.imgur.com/fplkz4p.gifv",
                "https://i.imgur.com/J2qBqS8.gifv",
                "https://i.imgur.com/utmXEEv.gifv",
                "https://i.imgur.com/jrjz2so.gifv",
                "https://i.imgur.com/qfRJLsa.gifv",
                "https://i.imgur.com/yRNSnc1.gifv",
                "https://i.imgur.com/uUniBJT.gifv",
                "https://i.imgur.com/Ar16njQ.gifv",
                "https://i.imgur.com/3mRgTWF.gifv",
                "https://i.imgur.com/wY4e3lR.gifv",
                "https://i.imgur.com/NSlAHPI.gifv",
                "https://i.imgur.com/7HJmcqk.gifv",
                "https://i.imgur.com/C7kbcFi.gifv",
                "https://i.imgur.com/gjLKPf0.gifv",
                "https://i.imgur.com/MJTl89b.gifv",
                "https://i.imgur.com/8zL6xB7.gifv",
                "https://i.imgur.com/uHKM4B1.gifv",
                "https://i.imgur.com/extkAtE.gifv",
                "https://i.imgur.com/4zEYGaZ.gifv",
                "https://i.imgur.com/SB9dq5K.gifv",
                "https://i.imgur.com/jPXH0lo.gifv",
                "https://i.imgur.com/3NnbuLF.gifv",
                "https://i.imgur.com/0kyY2rC.gifv",
                "https://i.imgur.com/f2grYDw.gifv",
                "https://i.imgur.com/h9UXEwT.gifv",
                "https://i.imgur.com/3bXnyqD.gifv",
                "https://i.imgur.com/vsJxAaC.gifv",
                "https://i.imgur.com/5FdBEdC.gifv",
                "https://i.imgur.com/8rOZJTn.gifv",
                "https://i.imgur.com/KjiIRzO.gifv",
                "https://i.imgur.com/1kBXbDA.gifv",
                "https://i.imgur.com/hdrCu7Y.gifv",
                "https://i.imgur.com/X29zFH1.gifv",
                "https://i.imgur.com/6mUbqVD.gifv",
                "https://i.imgur.com/i6nj5CK.gifv",
                "https://i.imgur.com/vbKaNrh.gifv",
                "https://i.imgur.com/5n7wgLd.gifv",
                "https://i.imgur.com/ujBvf9G.gifv",
                "https://i.imgur.com/c9Z8Hma.gifv",
                "https://i.imgur.com/jv8tJ5w.gifv",
                "https://i.imgur.com/ZUbHeKk.gifv",
                "https://i.imgur.com/pbhTxlT.gifv",
                "https://i.imgur.com/eW5zoJi.gifv",
                "https://i.imgur.com/cvfUsPh.gifv",
                "https://i.imgur.com/w741AGX.gifv",
                "https://i.imgur.com/M6GcP3V.gifv",
                "https://i.imgur.com/zLpRBFe.gifv",
                "https://i.imgur.com/sRIX7KW.gifv",
                "https://i.imgur.com/M3L1E7R.gifv",
                "https://i.imgur.com/cguzUir.gifv",
                "https://i.imgur.com/Xgats5l.gifv",
                "https://i.imgur.com/mFqUtIC.gifv",
                "https://i.imgur.com/qBlKVbM.gifv",
                "https://i.imgur.com/XIdqdzw.gifv",
                "https://i.imgur.com/A1Zk1Qz.gifv",
                "https://i.imgur.com/OJxF6Br.gifv",
                "https://i.imgur.com/ysHRiAJ.gifv",
                "https://i.imgur.com/IHDSVVf.gifv",
                "https://i.imgur.com/IqpsIUE.gifv",
                "https://i.imgur.com/1t9hkWe.gifv",
                "https://i.imgur.com/ewQkj9N.gifv",
                "https://i.imgur.com/CCXIa7S.gifv",
                "https://i.imgur.com/0z0olKt.gifv",
                "https://i.imgur.com/KpD5NPv.gifv",
                "https://i.imgur.com/jP5aSFH.gifv",
                "https://i.imgur.com/KIPyEGY.gifv",
                "https://i.imgur.com/TbpTkbH.gifv",
                "https://i.imgur.com/npuSUGu.gifv",
                "https://i.imgur.com/BlJ2TL0.gifv",
                "https://i.imgur.com/bMv9733.gifv",
                "https://i.imgur.com/nzzrEKe.gifv",
                "https://i.imgur.com/wPGILDt.gifv",
                "https://i.imgur.com/nUNPvSg.gifv",
                "https://i.imgur.com/u8sLnIl.gifv",
                "https://i.imgur.com/l7JSY23.gifv",
                "https://i.imgur.com/0aNAe7x.gifv",
                "https://i.imgur.com/B5H4dV5.gifv",
                "https://i.imgur.com/c5cKeZI.gifv",
                "https://i.imgur.com/DehEzbj.gifv",
                "https://i.imgur.com/53ICRY5.gifv",
                "https://i.imgur.com/F42h0ZG.gifv",
                "https://i.imgur.com/Z7X0jMp.gifv",
                "https://i.imgur.com/k54GQEP.gifv",
                "https://i.imgur.com/xvviuJm.gifv",
                "https://i.imgur.com/E34nY0p.gifv",
                "https://i.imgur.com/VQLdJ0W.gifv",
                "https://i.imgur.com/I88yU66.gifv",
                "https://i.imgur.com/TDFbtqD.gifv",
                "https://i.imgur.com/pp72CVf.gifv",
                "https://i.imgur.com/MfJxukv.gifv",
                "https://i.imgur.com/1JypEB2.gifv",
                "https://i.imgur.com/kLHAXVP.gifv",
                "https://i.imgur.com/xOHDRJu.gifv",
                "https://i.imgur.com/kSZO1Bw.gifv",
                "https://i.imgur.com/sfRXNPF.gifv",
                "https://i.imgur.com/fyuLKDz.gifv",
                "https://i.imgur.com/rN0wyEi.gifv",
                "https://i.imgur.com/MpGcmjU.gifv",
                "https://i.imgur.com/vWgObLD.gifv",
                "https://i.imgur.com/XAel22r.gifv",
                "https://i.imgur.com/zMVOsCG.gifv",
                "https://i.imgur.com/M8hxzWF.gifv",
                "https://i.imgur.com/DBVKu3N.gifv",
                "https://i.imgur.com/i4SH4Cd.gifv",
                "https://i.imgur.com/uBGtaF0.gifv",
                "https://i.imgur.com/rKwcRQl.gifv",
                "https://i.imgur.com/Y87M7aP.gifv",
                "https://i.imgur.com/SmMlmwm.gifv",
                "https://i.imgur.com/nvtlUpc.gifv",
                "https://i.imgur.com/U8ZnQ9K.gifv",
                "https://i.imgur.com/UH1MPDz.gifv",
                "https://i.imgur.com/i7pRJky.gifv",
                "https://i.imgur.com/R6cAVqR.gifv",
                "https://i.imgur.com/kFeKqgY.gifv",
                "https://i.imgur.com/KmtFX7O.gifv",
                "https://i.imgur.com/XXHIYPr.gifv",
                "https://i.imgur.com/ByYvvdD.gifv",
                "https://i.imgur.com/en710EQ.gifv",
                "https://i.imgur.com/BwjdO2q.gifv",
                "https://i.imgur.com/wo5Xfud.gifv",
                "https://i.imgur.com/UWm5V2q.gifv",
                "https://i.imgur.com/99mA3ek.gifv",
                "https://i.imgur.com/IGgQvjM.gifv",
                "https://i.imgur.com/XVcb04i.gifv",
                "https://i.imgur.com/yWxOIAT.gifv",
                "https://i.imgur.com/32BylPq.gifv",
                "https://i.imgur.com/Ll0W2LS.gifv",
                "https://i.imgur.com/GWFXxGy.gifv",
                "https://i.imgur.com/9UcNqzT.gifv",
                "https://i.imgur.com/G2eogqR.gifv",
                "https://i.imgur.com/yl78jZa.gifv",
                "https://i.imgur.com/LOKAEFk.gifv",
                "https://i.imgur.com/3tVUJkm.gifv",
                "https://i.imgur.com/Gyn3f3T.gifv",
                "https://i.imgur.com/Xw7qLsE.gifv",
                "https://i.imgur.com/oBhqalB.gifv",
                "https://i.imgur.com/XjK4lrM.gifv",
                "https://i.imgur.com/JytfhwO.gifv",
                "https://i.imgur.com/x6eK1vk.gifv",
                "https://i.imgur.com/Ojjn2Nh.gifv",
                "https://i.imgur.com/6bsAdft.gifv",
                "https://i.imgur.com/HSJlvSB.gifv",
                "https://i.imgur.com/R2WEiWM.gifv",
                "https://i.imgur.com/ugDT4pL.gifv",
                "https://i.imgur.com/AbNNLUW.gifv",
                "https://i.imgur.com/a0H3fKd.gifv",
                "https://i.imgur.com/B8rYdBV.gifv",
                "https://i.imgur.com/8LCX2UD.gifv",
                "https://i.imgur.com/xExWnYS.gifv",
                "https://i.imgur.com/zmK7l3a.gifv",
                "https://i.imgur.com/CDGwxyJ.gifv",
                "https://i.imgur.com/jvv3Gwd.gifv",
                "https://i.imgur.com/ARrRLPW.gifv"
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
