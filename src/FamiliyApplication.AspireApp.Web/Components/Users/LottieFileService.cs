namespace FamiliyApplication.AspireApp.Web.Components.Users
{
    public class LottieFileService 
    {

        private readonly LottieFileRec _emptyRec = new LottieFileRec("Finner ingenting", "/lottie/lottieempty.json", true);

        private readonly LottieFileRec[] _lottieRecs = new[]
        {
            new LottieFileRec("Jente med mobil", "/lottie/lottieprofiles/lottiejentemedmobil.json",true),
            new LottieFileRec("Jente som balanserer", "/lottie/lottieprofiles/lottiejentebalanserer.json",true),
            new LottieFileRec("Dame som blinker", "/lottie/lottieprofiles/lottiedameblinker.json",true),
            new LottieFileRec("Jente som vinker", "/lottie/lottieprofiles/lottiejentevinker.json",true),
            new LottieFileRec("Mann som går", "/lottie/lottieprofiles/lottisterkmanngår.json",true),
            new LottieFileRec("Gutt som hopper", "/lottie/lottieprofiles/lottieguttsomhopper.json",true),
            new LottieFileRec("Jente med hoodie", "/lottie/lottieprofiles/lottiehacker.json",true),
            new LottieFileRec("Jente som mediterer", "/lottie/lottieprofiles/lottiejentemediter.json",true),
            new LottieFileRec("Jente som danser", "/lottie/lottieprofiles/lottiejentedanser.json",true),
            new LottieFileRec("Mamma og 1 baby", "/lottie/lottieprofiles/mom1baby.json",true),
            new LottieFileRec("Mamma og 3 barn + katt", "/lottie/lottieprofiles/momand3kidsandcat.json",true),
            new LottieFileRec("Gutt", "/lottie/lottieprofiles/boy.json",true),
            new LottieFileRec("Gutt som svømmer", "/lottie/lottieprofiles/boyswimming.json",true),
            new LottieFileRec("Gutt som vinker", "/lottie/lottieprofiles/boywawing.json",true),
            new LottieFileRec("Pappa som tenker", "/lottie/lottieprofiles/dadthinking.json",true),
            new LottieFileRec("Fjeset til jente", "/lottie/lottieprofiles/girlface.json",true),
            new LottieFileRec("Jente som svever", "/lottie/lottieprofiles/girlfloating.json",true),
            new LottieFileRec("Jente som hopper", "/lottie/lottieprofiles/girljumpingafterberries.json",true),
            new LottieFileRec("Jente på moped", "/lottie/lottieprofiles/girlmoped.json",true),
            new LottieFileRec("Jente som hører på musikk", "/lottie/lottieprofiles/girlmusic.json",true),
            new LottieFileRec("Mamma som lager ett hjem", "/lottie/lottieprofiles/momcreatingahome.json",true),
            new LottieFileRec("Mamma med blomster", "/lottie/lottieprofiles/momflower.json",true),
            new LottieFileRec("Mamma som leser en bok", "/lottie/lottieprofiles/momreadingbook.json",true),
            new LottieFileRec("Mamma som går", "/lottie/lottieprofiles/momwalking.json",true),
        };



        public LottieFileRec[] GetLottieRecords()
        {
            return _lottieRecs;
        }

        public LottieFileRec GetLottieEmpty()
        {
            return _emptyRec;
        }
    }
}
