using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LaserIntFac;
using LaserIntFac.defing;
using Math;
using FrentoSecond.define;
using Newtonsoft.Json;
using RestSharp;

namespace FrentoSecond
{
    public class FrentoSecond : ILaser
    {
        ///////////////////////////////////////
        ///             Para                ///
        ///////////////////////////////////////

        #region Para

        public enum ParaType
        {
            Basic, Regiseter, Advence,
        }

        string RegisterAddress = "http://192.168.244.10:20060";
        string BasicAddress = "http://192.168.244.10:20022";           // We will need to alternate between REST clients, to be able to address both the standart request and the registers.


        private RestSharp.RestClient gRegisterClient;
        //
        int gPresentIndex = -1;

        define.Info gInfo;

        // Math
        Math.Math IMath = new Math.Math();

        // 
        string gPowerTable_path = "";
        string gLaserSet_path = "";
        
        #endregion


        ///////////////////////////////////////
        ///             Even                ///
        ///////////////////////////////////////

        #region Even

        // del
        public delegate void delErrMegBack(string code);
        public delegate void delErrCodeBack(int code);

        // Even
        public event delErrMegBack e_ErrMegBack;
        public event delErrCodeBack e_ErrCodeBack;


        #endregion


        ///////////////////////////////////////
        ///             Moni                ///
        ///////////////////////////////////////

        #region Monitor

        Thread t_ErrMonitor;


        #endregion

        ///////////////////////////////////////
        ///             Subr                ///
        ///////////////////////////////////////
        ///

        #region Sub

        private bool GetREGPara(Int32 addr, out Int32 value)
        {
            bool ret = false;
            value = 0;
            string back = "";
            try
            {
                gRegisterClient = new RestSharp.RestClient(RegisterAddress);

                gRegisterClient.Encoding = Encoding.UTF8;

                var om = gRegisterClient.Execute(new RestRequest("v0/Register/" + addr.ToString(), Method.GET));
                back = om.Content == "" ? "空" : om.Content;
                var pm = SimpleJson.DeserializeObject<JsonObject>(om.Content);

                value = Convert.ToInt32(pm);
            }
            catch (Exception ee)
            {
                e_ErrMegBack?.Invoke("add:" + addr.ToString() + ", valuse:" + value.ToString()
                                    + "back:" + back + "\r\n"
                                    + ee.Message);
                return false;
            }
            return ret;
        }

        public bool GetREGPara(ParaType type, string addr, out Int32 value)
        {
            bool ret = false;
            value = 0;
            string back = "";
            string typename = "";
            var restClient = new RestClient(RegisterAddress);
            switch (type)
            {
                case ParaType.Basic:
                    restClient = new RestClient(BasicAddress);
                    typename = "Basic";
                    break;
                case ParaType.Regiseter:
                    restClient = new RestClient(RegisterAddress);
                    typename = "Register";
                    break;
                case ParaType.Advence:
                    restClient = new RestClient(BasicAddress);
                    typename = "Advanced";
                    break;

            }
            try
            {
                var restBasic = restClient.Execute(new RestRequest("v0/" + typename, Method.GET)); // The decimal number is a converted value of the 0x7D002300 register - Power Supply Control.
                var restBasic2 = SimpleJson.DeserializeObject<JsonObject>(restBasic.Content);
                //bool e = restBasic2[addr].GetType().IsArray;

                value = Convert.ToInt32(restBasic2[addr]);
            }
            catch (Exception ee)
            {
                e_ErrMegBack?.Invoke("add:" + addr.ToString() + ", valuse:" + value.ToString()
                                    + "back:" + back + "\r\n"
                                    + ee.Message);
                return false;
            }
            return ret;
        }
        public bool GetREGPara(ParaType type, string addr, out string value)
        {
            bool ret = false;
            value = "";
            string back = "";
            string typename = "";
            var restClient = new RestClient(RegisterAddress);
            switch (type)
            {
                case ParaType.Basic:
                    restClient = new RestClient(BasicAddress);
                    typename = "Basic";
                    break;
                case ParaType.Regiseter:
                    restClient = new RestClient(RegisterAddress);
                    typename = "Register";
                    break;
                case ParaType.Advence:
                    restClient = new RestClient(BasicAddress);
                    typename = "Advanced";
                    break;

            }
            try
            {
                var restBasic = restClient.Execute(new RestRequest("v0/" + typename, Method.GET)); // The decimal number is a converted value of the 0x7D002300 register - Power Supply Control.
                var restBasic2 = SimpleJson.DeserializeObject<JsonObject>(restBasic.Content);
                //bool e = restBasic2[addr].GetType().IsArray;

                value = restBasic2[addr].ToString();
            }
            catch (Exception ee)
            {
                e_ErrMegBack?.Invoke("add:" + addr.ToString() + ", valuse:" + value.ToString()
                                    + "back:" + back + "\r\n"
                                    + ee.Message);
                return false;
            }
            return ret;
        }
        public bool GetREGPara(ParaType type, string addr, out double value)
        {
            bool ret = true;
            value = 0;
            string back = "";
            string typename = "";
            var restClient = new RestClient(RegisterAddress);
            switch (type)
            {
                case ParaType.Basic:
                    restClient = new RestClient(BasicAddress);
                    typename = "Basic";
                    break;
                case ParaType.Regiseter:
                    restClient = new RestClient(RegisterAddress);
                    typename = "Register";
                    break;
                case ParaType.Advence:
                    restClient = new RestClient(BasicAddress);
                    typename = "Advanced";
                    break;

            }
            try
            {
                var restBasic = restClient.Execute(new RestRequest("v0/" + typename, Method.GET)); // The decimal number is a converted value of the 0x7D002300 register - Power Supply Control.
                var restBasic2 = SimpleJson.DeserializeObject<JsonObject>(restBasic.Content);
                //bool e = restBasic2[addr].GetType().IsArray;

                value = Convert.ToDouble(restBasic2[addr]);
            }
            catch (Exception ee)
            {
                e_ErrMegBack?.Invoke("add:" + addr.ToString() + ", valuse:" + value.ToString()
                                    + "back:" + back + "\r\n"
                                    + ee.Message);
                return false;
            }
            return ret;
        }


        private bool GetREGPara(Int32 addr, out float value)
        {
            bool ret = false;
            value = 0;
            try
            {
                gRegisterClient = new RestSharp.RestClient(RegisterAddress);
                var om = gRegisterClient.Execute(new RestRequest("v0/Register/" + addr.ToString(), Method.GET));
                var pm = SimpleJson.DeserializeObject<JsonObject>(om.Content);

                value = (float)Convert.ToDecimal(pm);
            }
            catch (Exception ee)
            {
                e_ErrMegBack?.Invoke("GetREGPara fail\r\n" + ee.Message);
                return false;
            }
            return ret;
        }

        private bool SetREGPara(Int32 addr, Int32 valuse)
        {
            try
            {

                var om = new RestRequest("v0/Register/" + addr.ToString(), Method.PUT);
                om.RequestFormat = DataFormat.Json;
                om.AddBody(valuse);

            }
            catch (Exception ee)
            {
                e_ErrMegBack?.Invoke("SetREGPara fail, add:" + addr.ToString() + ", valuse:" + valuse.ToString() + "\r\n"
                                    + ee.Message);
                return false;
            }
            return true;
        }

        public bool SetREGParaPut(ParaType type, string addr, Int32 valuse)
        {
            var restClient = new RestClient(RegisterAddress);
            string typename = "";
            switch (type)
            {
                case ParaType.Basic:
                    restClient = new RestClient(BasicAddress);
                    typename = "Basic";
                    break;
                case ParaType.Regiseter:
                    restClient = new RestClient(RegisterAddress);
                    typename = "Register";
                    break;
                case ParaType.Advence:

                    typename = "Advanced";
                    break;
            }
            try
            {

                var om = new RestRequest("v0/" + typename + '/' + addr.ToString(), Method.PUT);
                om.RequestFormat = DataFormat.Json;
                om.AddBody(valuse);

                IRestResponse oim = restClient.Execute(om);
                if (oim.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception("put add: " + addr + ", value:" + valuse);
            }
            catch (Exception ee)
            {
                e_ErrMegBack?.Invoke("SetREGPara fail, add:" + addr.ToString() + ", valuse:" + valuse.ToString() + "\r\n"
                                    + ee.Message);
                return false;
            }
            return true;
        }
        public bool SetREGParaPut(ParaType type, string addr, double valuse)
        {
            var restClient = new RestClient(RegisterAddress);
            string typename = "";
            switch (type)
            {
                case ParaType.Basic:
                    restClient = new RestClient(BasicAddress);
                    typename = "Basic";
                    break;
                case ParaType.Regiseter:
                    restClient = new RestClient(RegisterAddress);
                    typename = "Register";
                    break;
                case ParaType.Advence:

                    typename = "Advanced";
                    break;
            }
            try
            {

                var om = new RestRequest("v0/" + typename + '/' + addr.ToString(), Method.PUT);
                om.RequestFormat = DataFormat.Json;
                om.AddBody(valuse);

                IRestResponse oim = restClient.Execute(om);
                if (oim.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception("put add: " + addr + ", value:" + valuse);
            }
            catch (Exception ee)
            {
                e_ErrMegBack?.Invoke("SetREGPara fail, add:" + addr.ToString() + ", valuse:" + valuse.ToString() + "\r\n"
                                    + ee.Message);
                return false;
            }
            return true;
        }
        public bool SetREGParaPost(ParaType type, string addr, Int32 valuse)
        {
            var restClient = new RestClient(RegisterAddress);
            string typename = "";
            switch (type)
            {
                case ParaType.Basic:
                    restClient = new RestClient(BasicAddress);
                    typename = "Basic";
                    break;
                case ParaType.Regiseter:
                    restClient = new RestClient(RegisterAddress);
                    typename = "Register";
                    break;
                case ParaType.Advence:

                    typename = "Advanced";
                    break;
            }
            try
            {

                var om = new RestRequest("v0/" + typename + '/' + addr.ToString(), Method.POST);

                IRestResponse oim = restClient.Execute(om);
                if (oim.StatusCode != System.Net.HttpStatusCode.OK)
                    throw new Exception("post add: " + addr + ", value:" + valuse);
            }
            catch (Exception ee)
            {
                e_ErrMegBack?.Invoke("SetREGPara fail, add:" + addr.ToString() + ", valuse:" + valuse.ToString() + "\r\n"
                                    + ee.Message);
                return false;
            }
            return true;
        }

        private void Monitor_Err()
        {
            GetREGPara(ParaType.Basic, Basic_Get.ERRORS, out int value);
            if (value != 0)
                e_ErrCodeBack?.Invoke(value);
        }

        #endregion

        ///////////////////////////////////////
        ///             Meth                ///
        ///////////////////////////////////////


        #region Method

        
        
        FrentoSecond(string path_PowerTable, string path_LaserSet)
        {
            gPowerTable_path = path_PowerTable;
            gLaserSet_path = path_LaserSet;
        }

        ///
        public RetErr Open()
        {
            RetErr ret = new RetErr();
            string tamp = "";
            try
            {
                #region ReadData

                string path = gLaserSet_path;
                if (!System.IO.File.Exists(path))
                {
                    // 建立檔案
                    gInfo = new define.Info();
                    string s = JsonConvert.SerializeObject(gInfo, Formatting.Indented);
                    System.IO.StreamWriter strw = new System.IO.StreamWriter(path, false, Encoding.Default);
                    strw.Write(s);
                    strw.Close();
                    throw new Exception("Can't Find file : " + path);
                }
                System.IO.StreamReader strr = new System.IO.StreamReader(path, Encoding.Default);
                string ori = strr.ReadToEnd();
                strr.Close();

                gInfo = JsonConvert.DeserializeObject<define.Info>(ori);

                gPresentIndex = gInfo.StartPrcent;
                #endregion

                #region  Set Global DATA

                RegisterAddress = "http://" + gInfo.IP;
                BasicAddress = "http://" + gInfo.IP;

                #endregion

                #region Monitor

                t_ErrMonitor = new Thread(Monitor_Err);
                t_ErrMonitor.Start();

                #endregion


            }
            catch (Exception ee)
            {
#if Log
                Log.Pushlist(Num._CheckLaserReady,
                                System.Reflection.MethodBase.GetCurrentMethod().ToString(),
                                ee.Message);
#endif
                ret.flag = false;
                ret.Meg = "FremtoSecond CheckLaserReady fail!" + ee.Message;
                ret.Num = Num._CheckLaserReady;
                return ret;
            }
            return ret;
        }
        public RetErr CheckHours()
        {
            RetErr ret = new RetErr();
            string tamp = "";
            try
            {


            }
            catch (Exception ee)
            {
#if Log
                Log.Pushlist(Num._CheckLaserReady,
                                System.Reflection.MethodBase.GetCurrentMethod().ToString(),
                                ee.Message);
#endif
                ret.flag = false;
                ret.Meg = "FremtoSecond CheckLaserReady fail!\r\n" + ee.Message;
                ret.Num = Num._CheckLaserReady;
                return ret;
            }
            return ret;
        }

        public RetErr CheckLaserEnabled(out EEnabledState PEnabledState)
        {
            RetErr ret = new RetErr();
            string tamp = "";
            PEnabledState = EEnabledState.esStandbyByFault;
            try
            {
                bool bret = GetREGPara(ParaType.Basic, Basic_Get.IsOutputEnable, out int value);
                if (!bret) throw new Exception("Get IsOutputEnable, fail");
                if (value == 1)
                    PEnabledState = EEnabledState.esEnabled;
                else
                    PEnabledState = EEnabledState.esStandby;


            }
            catch (Exception ee)
            {
#if Log
                Log.Pushlist(Num._CheckLaserReady,
                                System.Reflection.MethodBase.GetCurrentMethod().ToString(),
                                ee.Message);
#endif
                ret.flag = false;
                ret.Meg = "FremtoSecond CheckLaserReady fail!" + ee.Message;
                ret.Num = Num._CheckLaserReady;
                return ret;
            }
            return ret;
        }

        public RetErr CheckLaserFault()
        {
            RetErr ret = new RetErr();
            string tamp = "";
            try
            {
                bool bret = GetREGPara(ParaType.Basic, Basic_Get.ERRORS, out int value);
                if (!bret) throw new Exception("Get IsOutputEnable, fail");
                if (value != 0)
                    throw new Exception("ErrCode: " + value.ToString());

            }
            catch (Exception ee)
            {
#if Log
                Log.Pushlist(Num._CheckLaserReady,
                                System.Reflection.MethodBase.GetCurrentMethod().ToString(),
                                ee.Message);
#endif
                ret.flag = false;
                ret.Meg = "FremtoSecond CheckLaserReady fail!" + ee.Message;
                ret.Num = Num._CheckLaserReady;
                return ret;
            }
            return ret;
        }

        public RetErr CheckLaserReady()
        {
            RetErr ret = new RetErr();
            string tamp = "";
            try
            {
                // 是否通訊成功

                // 是否Preset
                GetREGPara(ParaType.Basic, Basic_Get.Act_StateName, out string state);
                if (!Equals(state, Basic_State.OperationalState))
                    throw new Exception("Laser State error, " + state);

                // 是否enable
                GetREGPara(ParaType.Basic, Basic_Get.IsOutputEnable, out int isenable);
                if (isenable != 1)
                    throw new Exception("Laser Not Enable");

            }
            catch (Exception ee)
            {
#if Log
                Log.Pushlist(Num._CheckLaserReady,
                                System.Reflection.MethodBase.GetCurrentMethod().ToString(),
                                ee.Message);
#endif
                ret.flag = false;
                ret.Meg = "FremtoSecond CheckLaserReady fail!" + ee.Message;
                ret.Num = Num._CheckLaserReady;
                return ret;
            }
            return ret;
        }

        public RetErr CheckShutterAndDIODEOpened()
        {
            RetErr ret = new RetErr();
            string tamp = "";
            try
            {



            }
            catch (Exception ee)
            {
#if Log
                Log.Pushlist(Num._CheckLaserReady,
                                System.Reflection.MethodBase.GetCurrentMethod().ToString(),
                                ee.Message);
#endif
                ret.flag = false;
                ret.Meg = "FremtoSecond CheckLaserReady fail!" + ee.Message;
                ret.Num = Num._CheckLaserReady;
                return ret;
            }
            return ret;
        }

        public RetErr Close()
        {
            RetErr ret = new RetErr();
            string tamp = "";
            try
            {
                // go to standby
                SetREGParaPost(ParaType.Basic, Basic_Post.GOTOSTANDBY, 1);
                // Monitor to standby
                GetREGPara(ParaType.Basic, Basic_Get.Act_StateName, out string state);
                while (!Equals(state, "StateStandingBy"))
                    GetREGPara(ParaType.Basic, Basic_Get.Act_StateName, out state);

                // Monitor
                t_ErrMonitor.Abort();

            }
            catch (Exception ee)
            {
#if Log
                Log.Pushlist(Num._CheckLaserReady,
                                System.Reflection.MethodBase.GetCurrentMethod().ToString(),
                                ee.Message);
#endif
                ret.flag = false;
                ret.Meg = "FremtoSecond CheckLaserReady fail!" + ee.Message;
                ret.Num = Num._CheckLaserReady;
                return ret;
            }
            return ret;
        }

        public RetErr GetBiasVoltage(out double pBiasVoltage)
        {
            RetErr ret = new RetErr();
            pBiasVoltage = 0;
            return ret;
        }

        public RetErr GetHours(out int pHours)
        {
            RetErr ret = new RetErr();
            pHours = 0;
            return ret;
        }

        public RetErr GetPatBins(out char asBins)
        {
            RetErr ret = new RetErr();
            asBins = ' ';
            return ret;
        }

        public RetErr GetPowerVoltage(out double pPowerVoltage)
        {
            RetErr ret = new RetErr();
            pPowerVoltage = 0;
            return ret;
        }

        public RetErr GetSpot(out int pSpot)
        {
            RetErr ret = new RetErr();
            pSpot = -1;
            return ret;
        }

        public RetErr GetTemperatrue(out double Temperature)
        {
            RetErr ret = new RetErr();
            Temperature = 0;
            return ret;
        }

        public RetErr HaveEPulse(out bool isSup)
        {
            RetErr ret = new RetErr();
            isSup = false;
            return ret;
        }

        public RetErr HaveWarmUp(out bool isSup)
        {
            RetErr ret = new RetErr();
            isSup = true;
            return ret;
        }

        public RetErr IsWarmingUp(int pSecs, out bool isOK)
        {
            RetErr ret = new RetErr();
            isOK = false;
            try
            {
                GetREGPara(ParaType.Basic, Basic_Get.Act_StateName, out string state1);
                if (Equals(state1, "StateEmissionOn"))
                    return ret;
                // select preset
                SetREGParaPut(ParaType.Basic, Basic_Put.Select_PRSET_Index, gPresentIndex);

                // execute
                SetREGParaPost(ParaType.Basic, Basic_Post.PRSETSELECT, 1);

                GetREGPara(ParaType.Basic, Basic_Get.Act_StateName, out string state);
                while (!Equals(state, "StateEmissionOn"))
                {
                    GetREGPara(ParaType.Basic, Basic_Get.Act_StateName, out state);
                    Console.WriteLine(state);
                    if (state == "StateFailure")
                        throw new Exception("Laser Warming Up fail");
                    Thread.Sleep(pSecs * 1000);
                }


                isOK = true;
            }
            catch (Exception ee)
            {
#if Log
                Log.Pushlist(Num._CheckLaserReady,
                                System.Reflection.MethodBase.GetCurrentMethod().ToString(),
                                ee.Message);
#endif
                ret.flag = false;
                ret.Meg = "FremtoSecond CheckLaserReady fail!" + ee.Message;
                ret.Num = Num._CheckLaserReady;
                return ret;
            }
            return ret;
        }


        public RetErr OpenShutterAndDIODE(bool POpen)
        {
            RetErr ret = new RetErr();
            string tamp = "";
            try
            {

                // select Prsent index
                //Log.Pushlist_debug("Enable On");
                SetREGParaPost(ParaType.Basic, Basic_Post.ENABLEOUTPUT, 1);


            }
            catch (Exception ee)
            {
#if Log
                Log.Pushlist(Num._CheckLaserReady,
                                System.Reflection.MethodBase.GetCurrentMethod().ToString(),
                                ee.Message);
#endif
                ret.flag = false;
                ret.Meg = "FremtoSecond CheckLaserReady fail!" + ee.Message;
                ret.Num = Num._CheckLaserReady;
                return ret;
            }
            return ret;
        }

        public RetErr SetBiasVoltage(double pBiasVoltage)
        {
            RetErr ret = new RetErr();
            return ret;
        }

        public RetErr SetDiodeCurrent(double PDCurr)
        {
            RetErr ret = new RetErr();
            return ret;
        }

        public RetErr SetEPulse(double PEPulse)
        {
            RetErr ret = new RetErr();
            return ret;
        }

        public RetErr SetETime(int etime)
        {
            RetErr ret = new RetErr();
            return ret;
        }

        public RetErr SetGate(bool POpen)
        {
            RetErr ret = new RetErr();
            return ret;
        }

        public RetErr SetLaserPreset(double PPower, double PFreq, double PMove, double PBias, double PEpulse)
        {
            RetErr ret = new RetErr();
            string tamp = "";
            bool ret_s = true;
            try
            {
                #region Power

                // Set
                IMath.PowerConvert(gPowerTable_path, PPower, out double _Power);
                ret_s = SetREGParaPut(ParaType.Basic, Basic_Put.Tar_Attenuator_Percent, _Power);
                if (!ret_s) throw new Exception("Set Target Power Percent fail, " + _Power.ToString());


                #endregion

                #region freq
                ret_s = GetREGPara(ParaType.Basic, Basic_Get.Act_Output_Freq, out double act_Freq);
                if (!ret_s) throw new Exception("Get Act Freq fail, " + act_Freq.ToString());
                ret_s = GetREGPara(ParaType.Basic, Basic_Get.Act_PP_Divider, out double _PP);
                if (!ret_s) throw new Exception("Get Act_PP_Divider fail, " + _PP.ToString());

                double basicFreq = act_Freq * _PP;
                double divPP = basicFreq / PFreq;

                ret_s = SetREGParaPut(ParaType.Basic, Basic_Put.Tar_PP_Divider, divPP);
                if (!ret_s) throw new Exception("Set Target PP_Divider fail, " + divPP.ToString());



                #endregion

                #region chk

                // Power
                ret_s = GetREGPara(ParaType.Basic, Basic_Get.Act_Attenuator_Percent, out double power);
                if (!ret_s) throw new Exception("Get Act Power Percent fail, " + power.ToString());
                while (!(System.Math.Abs(power - _Power) < 0.1))
                {
                    ret_s = GetREGPara(ParaType.Basic, Basic_Get.Act_Attenuator_Percent, out power);
                    if (!ret_s) throw new Exception("Get Act Power Percent fail, " + power.ToString());
                }

                // Freq
                ret_s = GetREGPara(ParaType.Basic, Basic_Get.Act_PP_Divider, out double _pp);
                if (!ret_s) throw new Exception("Get Act PP_Divider fail, " + _pp.ToString());
                #endregion

            }
            catch (Exception ee)
            {
#if Log
                Log.Pushlist(Num._CheckLaserReady,
                                System.Reflection.MethodBase.GetCurrentMethod().ToString(),
                                ee.Message);
#endif
                ret.flag = false;
                ret.Meg = "FremtoSecond CheckLaserReady fail!" + ee.Message;
                ret.Num = Num._CheckLaserReady;
                return ret;
            }
            return ret;
        }

        public RetErr SetMode(int pMode)
        {
            RetErr ret = new RetErr();
            return ret;
        }

        public RetErr SetPatternBin(int patbin)
        {
            RetErr ret = new RetErr();
            return ret;
        }

        public RetErr SetPowerAndFreq(double PPower, double PFreq)
        {
            RetErr ret = new RetErr();
            string tamp = "";
            bool ret_s = true;
            try
            {
                #region Power
                double _Power = PPower;
                // Set
                ret_s = SetREGParaPut(ParaType.Basic, Basic_Put.Tar_Attenuator_Percent, _Power);
                if (!ret_s) throw new Exception("Set Target Power Percent fail, " + _Power.ToString());


                #endregion

                #region freq
                ret_s = GetREGPara(ParaType.Basic, Basic_Get.Act_Output_Freq, out double act_Freq);
                if (!ret_s) throw new Exception("Get Act Freq fail, " + act_Freq.ToString());
                ret_s = GetREGPara(ParaType.Basic, Basic_Get.Act_PP_Divider, out double _PP);
                if (!ret_s) throw new Exception("Get Act_PP_Divider fail, " + _PP.ToString());

                double basicFreq = act_Freq * _PP;
                double divPP = basicFreq / PFreq;

                ret_s = SetREGParaPut(ParaType.Basic, Basic_Put.Tar_PP_Divider, divPP);
                if (!ret_s) throw new Exception("Set Target PP_Divider fail, " + divPP.ToString());



                #endregion

                #region chk

                // Power
                ret_s = GetREGPara(ParaType.Basic, Basic_Get.Act_Attenuator_Percent, out double power);
                if (!ret_s) throw new Exception("Get Act Power Percent fail, " + power.ToString());
                while (!(System.Math.Abs(power - _Power) < 0.1))
                {
                    ret_s = GetREGPara(ParaType.Basic, Basic_Get.Act_Attenuator_Percent, out power);
                    if (!ret_s) throw new Exception("Get Act Power Percent fail, " + power.ToString());
                }

                // Freq
                ret_s = GetREGPara(ParaType.Basic, Basic_Get.Act_PP_Divider, out double _pp);
                if (!ret_s) throw new Exception("Get Act PP_Divider fail, " + _pp.ToString());

                #endregion


            }
            catch (Exception ee)
            {
#if Log
                Log.Pushlist(Num._CheckLaserReady,
                                System.Reflection.MethodBase.GetCurrentMethod().ToString(),
                                ee.Message);
#endif
                ret.flag = false;
                ret.Meg = "FremtoSecond CheckLaserReady fail!" + ee.Message;
                ret.Num = Num._CheckLaserReady;
                return ret;
            }
            return ret;
        }

        public RetErr Shutdown()
        {
            RetErr ret = new RetErr();
            bool ret_s = true;
            try
            {
                ret_s = SetREGParaPost(ParaType.Basic, Basic_Post.GOTOSTANDBY, 1);
                if (!ret_s) throw new Exception("Get to standby fail");

                ret_s = GetREGPara(ParaType.Basic, Basic_Get.Act_StateName, out string state);
                if (!ret_s) throw new Exception("Get state fail");
                while (Equals(state, Basic_State.StandByState))
                {
                    ret_s = GetREGPara(ParaType.Basic, Basic_Get.Act_StateName, out state);
                    if (!ret_s) throw new Exception("Get state fail");
                }


            }
            catch (Exception ee)
            {
#if Log
                Log.Pushlist(Num._CheckLaserReady,
                                System.Reflection.MethodBase.GetCurrentMethod().ToString(),
                                ee.Message);
#endif
                ret.flag = false;
                ret.Meg = "FremtoSecond CheckLaserReady fail!" + ee.Message;
                ret.Num = Num._CheckLaserReady;
                return ret;
            }
            return ret;
        }

        public RetErr Standby()
        {
            RetErr ret = new RetErr();
            return ret;
        }


        public bool CmdSend(int addr, Int32 value)
        {
            return true;
        }

        public bool CmdBack(int addr, out int value)
        {
            value = 0;
            return true;
        }

        #endregion
    }
}


namespace FrentoSecond.define
{
    public static class Basic_State
    {
        public static string HousekeepingState = "Housekeeping";                                         // Setting the HousekeepingState variable to a strin Housekeeping
        public static string OperationalState = "Operational";                                           // Setting the OperationalState variable to a strin Operational
        public static string StandByState = "Standby";                                           // Setting the OperationalState variable to a strin Operational
    }
    public static class Basic_Post
    {

        public static string PRSETSELECT = "ApplySelectedPreset";
        public static string CLOSEOUTPUT = "CloseOutput";
        public static string ENABLEOUTPUT = "EnableOutput";
        public static string GOTOSTANDBY = "GoToStandby";

    }

    public static class Basic_Get
    {
        public static string ERRORS = "Errors";
        public static string GENERALSTATUS = "GeneralStatus";
        public static string IsOutputEnable = "IsOutputEnabled";
        public static string Select_PRSET_Index = "SelectedPresetIndex";
        public static string Tar_Attenuator_Percent = "TargetAttenuatorPercentage";
        public static string Tar_PP_Divider = "TargetPpDivider";
        public static string WARNING = "Warnings";
        public static string Act_RA_FREQ = "GetActualRaFrequency";
        public static string Act_StateName = "ActualStateName";
        public static string Act_RA_Power = "ActualRaPower";
        public static string Act_PP_Divider = "ActualPpDivider";
        public static string Act_Output_Power = "ActualOutputPower";
        public static string Act_Output_Freq = "ActualOutputFrequency";
        public static string Act_Output_Energy = "ActualOutputEnergy";
        public static string Act_Attenuator_Percent = "ActualAttenuatorPercentage";
    }

    public static class Basic_Put
    {
        public static string Select_PRSET_Index = "SelectedPresetIndex";
        public static string Tar_Attenuator_Percent = "TargetAttenuatorPercentage";
        public static string Tar_PP_Divider = "TargetPpDivider";
    }

    public static class Advenced_Get
    {
        public static string Presets = "Presets";

    }


    struct info_EtherNet
    {
        string IP;
    }

    class Info
    {
        public string IP = "192.168.244.10:20022";
        public int Max_Preset;
        public int Min_Preset;
        public int StartPrcent;

    }
}
