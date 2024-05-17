using nanoFramework.Json;
using nanoFramework.Networking;
using System;
using System.Device.Adc;
using System.Diagnostics;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading;

namespace PostAdcData
{
    public class Program
    {
        // WiFi
        private static string MySsid = "SSID";
        private static string MyPassword = "WIFI PASSWORD";

        // ������H�̒�R�l
        private static int R1 = 10000;
        private static int R2 = 10000;

        // ���M�Ԋu
        private static int Interval = 60 * 1000;

        public static void Main()
        {
            ConnectWiFi();

            while (true)
            {
                try
                {
                    VoltData voltData = ReadAdc();
                    SendData(voltData);

                    Thread.Sleep(Interval);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    break;
                }
            }

            Thread.Sleep(Timeout.Infinite);
        }

        /// <summary>
        /// WiFi�ɐڑ����܂��B
        /// </summary>
        private static void ConnectWiFi()
        {
            Debug.WriteLine("Waiting for network up...");

            bool success;
            CancellationTokenSource cs = new(60000);

            success = WifiNetworkHelper.ConnectDhcp(MySsid, MyPassword, requiresDateTime: true, token: cs.Token);

            if (!success)
            {
                if (NetworkHelper.HelperException != null)
                {
                    Debug.WriteLine($"Exception: {NetworkHelper.HelperException}");
                }
                return;
            }

            NetworkInterface networkInterface = NetworkInterface.GetAllNetworkInterfaces()[0];

            string ipAddress = networkInterface.IPv4Address.ToString();
            string gatewayAddress = networkInterface.IPv4GatewayAddress.ToString();

            Debug.WriteLine($"Network ready. IP address: {ipAddress}, Gateway: {gatewayAddress}");
        }

        /// <summary>
        /// �d���̓ǂݎ����s���܂��B
        /// </summary>
        /// <returns>�ǂݎ�茋�ʂ�Ԃ��܂��B</returns>
        private static VoltData ReadAdc()
        {
            AdcController adc1 = new AdcController();
            AdcChannel channel6 = adc1.OpenChannel(6); // GPIO34

            int adcValue = channel6.ReadValue();
            double voltage = adcValue * (R1 + R2) / R2 * (3.3 / 4095);

            Debug.WriteLine($"ADC Value: {adcValue}");
            Debug.WriteLine($"Voltage: {voltage}");

            VoltData voltData = new VoltData(adcValue, voltage);

            return voltData;
        }

        /// <summary>
        /// �f�[�^�̑��M���s���܂��B
        /// </summary>
        /// <param name="voltData">�d���f�[�^���w�肵�܂��B</param>
        private static void SendData(VoltData voltData)
        {
            string json = JsonSerializer.SerializeObject(voltData);
            StringContent content = new StringContent(json);

            HttpClient httpClient = new HttpClient();
            httpClient.Post("[POST TO SERVER]", content);
        }
    }
}
