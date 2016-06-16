using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Band;
using Microsoft.Band.Sensors;
using Microsoft.Band.Tiles.Pages;
using Microsoft.Band.Tiles;
using Windows.UI.Xaml.Media.Imaging;
using Microsoft.Band.Notifications;
//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace BandSample1
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        IBandInfo[] pairedBands;
        bool LinkOkFlag = false;
        IBandClient bandClient;
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btSearchBand_Click(object sender, RoutedEventArgs e)
        {
            pairedBands = await
                BandClientManager.Instance.GetBandsAsync();

            try
            {
                if (pairedBands[0].Name.Length > 0)
                {
                    cbBandName.Items.Add(pairedBands[0].Name);
                    tbLinkModel.Text = "Band been search.";
                    cbBandName.SelectedIndex = 0;
                }
                else
                {
                    tbLinkModel.Text = "Search failed,try again.";
                }
            }
            catch
            {
                tbLinkModel.Text = "ERROR";
            }

        }

        private async void btLinkTheBand_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (bandClient = await
                    BandClientManager.Instance.ConnectAsync(pairedBands[0]))
                {
                    tbLinkModel.Text = "Link Succeed!";// do work after successful connect
                    LinkOkFlag = true;
                }
            }
            catch (BandException ex)
            {
                // handle a Band connection exception }
            }
        }

        private async void btTest_Click(object sender, RoutedEventArgs e)
        {
            //确认用户是否同意获取讯息
            if (bandClient.SensorManager.Accelerometer.GetCurrentUserConsent() != UserConsent.Granted)
            {
                await
                 //请求用户同意进行异步
                 bandClient.SensorManager.Accelerometer.RequestUserConsentAsync();
            }
           
                await 
                bandClient.SensorManager.Accelerometer.StartReadingsAsync();
            
        }

        private async void btReadVersion_Click(object sender, RoutedEventArgs e)
        {
            string fwVersion = "";
            string hwVersion = "";
            try
            {
                fwVersion = await bandClient.GetFirmwareVersionAsync();
                hwVersion = await bandClient.GetHardwareVersionAsync();
                tbVersion.Text = "Band FirmWare Version: " + fwVersion;

                // do work with firmware & hardware versions
            }
            catch (BandException ex)
            {
                // handle any BandExceptions
            }

        }

        private void btSendTile_Click(object sender, RoutedEventArgs e)
        {
            //向手表发送自制磁贴


        }

        private async void btGetTileMessage_Click(object sender, RoutedEventArgs e)
        {
            try
            { // get the current set of tiles
                IEnumerable<BandTile> tiles = await
                    bandClient.TileManager.GetTilesAsync();
            }
            catch (BandException ex)
            {
                // handle a Band connection exception 
            }
        }

        private async void btSendMessage_Click(object sender, RoutedEventArgs e)
        {

            Guid tileGuid = Guid.NewGuid();
            WriteableBitmap smallIconBitmap = new WriteableBitmap(24, 24);
            BandIcon smallIcon = smallIconBitmap.ToBandIcon();

            WriteableBitmap tileIconBitmap = new WriteableBitmap(46, 46);
            BandIcon tileIcon = tileIconBitmap.ToBandIcon();


            BandTile tile = new BandTile(tileGuid)
            { // enable badging (the count of unread messages) IsBadgingEnabled = true, 
              // set the name
                Name = "TileName",
                // set the icons
                SmallIcon = smallIcon,
                TileIcon = tileIcon
            };

            if (await bandClient.TileManager.AddTileAsync(tile))
            { // do work if the tile was successfully created }
                try
                { // send a dialog to the Band for one of our tiles
                    await
                     bandClient.NotificationManager.SendMessageAsync(tileGuid,
                                                                     "Message title",
                                                                     "Message body",
                                                                     DateTimeOffset.Now,
                                                                     MessageFlags.ShowDialog);
                }
                catch (BandException ex)
                { 
                    // handle a Band connection exception 
                }
            }
        }

        private void tbAcce_SelectionChanged(object sender, RoutedEventArgs e)
        {
            bandClient.SensorManager.Accelerometer.ReadingChanged += (sender, args) =>
            {

            };
        }
    }
}
