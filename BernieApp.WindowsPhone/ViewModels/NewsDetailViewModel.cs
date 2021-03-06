﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using BernieApp.Portable.Models;
using BernieApp.Portable.Client;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Popups;
using System.Diagnostics;
using BernieApp.WindowsPhone.Common;

namespace BernieApp.WindowsPhone.ViewModels
{
    public class NewsDetailViewModel : MainViewModel, INavigable
    {
        private INavigationService _navigationService;
        private FeedEntry _item = new FeedEntry();
        private RelayCommand _openWebPageCommand;
        private RelayCommand _shareCommand;

        public FeedEntry Item
        {
            get { return _item; }
            set { Set(ref _item, value); }
        }

        public NewsDetailViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;
        }

        public override void Activate(object parameter)
        {
            var entry = parameter as FeedEntry;
            if (entry != null)
            {
                _item.Id = entry.Id;
                _item.Title = entry.Title;
                _item.ArticleType = entry.ArticleType;
                _item.Date = entry.Date;
                _item.Body = entry.Body;
                _item.Url = entry.Url;
                _item.ImageUrl = entry.ImageUrl;
            }

            //Register for share
            DataTransferManager.GetForCurrentView().DataRequested += OnShareDataRequested;
        }

        public override void Deactivate(object parameter)
        {
            //Un-register for share
            DataTransferManager.GetForCurrentView().DataRequested -= OnShareDataRequested;
        }

        //Open article Url in Web Browser
        public RelayCommand OpenWebPageCommand
        {
            get
            {
                if (_openWebPageCommand == null)
                {
                    _openWebPageCommand = new RelayCommand(async () =>
                    {
                        var success = await Launcher.LaunchUriAsync(new Uri(Item.Url));
                        if (success)
                        {
                            Debug.WriteLine("url successfully opened in browser");
                        }
                        else
                        {
                            var dialog = new MessageDialog("Unable to open the webpage, please try again later.", "Oops...");
                            await dialog.ShowAsync();
                        }
                    });
                }
                return _openWebPageCommand;
            }
        }

        //Invoke Share charm to share a link to the article
        public RelayCommand ShareCommand
        {
            get
            {
                if (_shareCommand == null)
                {
                    _shareCommand = new RelayCommand(() =>
                    {
                        DataTransferManager.ShowShareUI();
                    });
                }
                return _shareCommand;
            }
        }

        void OnShareDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var request = args.Request;

            request.Data.Properties.Title = " ";
            request.Data.Properties.Description = "Share this news article";
            request.Data.SetWebLink(new Uri(Item.Url));
        }
    }
}
