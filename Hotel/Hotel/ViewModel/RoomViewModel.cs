﻿using Hotel.Model;
using Hotel.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Hotel.ViewModel
{
    internal class RoomViewModel : BaseViewModel
    {
        private int idnv;
        private ObservableCollection<RoomVM> _roomListdb;
        private ObservableCollection<RoomVM> _roomList;
        public ObservableCollection<RoomVM> RoomList
        {
            get { return _roomList; }
            set { _roomList = value; OnPropertyChanged(); }
        }
        private ComboBoxItem _sortRoom;
        public ComboBoxItem SortRoom
        {
            get { return _sortRoom; }
            set
            {
                _sortRoom = value;
                OnPropertyChanged();
            }
        }
        private string _textToFilter;

        public string TextToFilter
        {
            get { return _textToFilter; }
            set
            {
                _textToFilter = value;
                OnPropertyChanged();
                RoomCollection.Filter = FilterByName;
            }
        }
        private ICollectionView _roomCollection;

        public ICollectionView RoomCollection
        {
            get { return _roomCollection; }
            set { _roomCollection = value; OnPropertyChanged(); }
        }
        public ICommand LoadIdStaff { get; set; }
        public ICommand btnAll { get; set; }
        public ICommand btnAvailabel { get; set; }
        public ICommand btnOrdered { get; set; }
        public ICommand btnRepair { get; set; }
        public ICommand sortRoom { get; set; }
        public ICommand choseRoom { get; set; }

        public RoomViewModel()
        {
            RoomList = new ObservableCollection<RoomVM>();
            LoadIdStaff = new RelayCommand<UserControl>((p) => true, (p) => { idnv = Convert.ToInt32(GetIdStaff(p)); });
            btnAll = new RelayCommand<object>((p) => true, (p) => LoadAllRoom());
            btnAvailabel = new RelayCommand<object>((p) => true, (p) => LoadAvailabel());
            btnOrdered = new RelayCommand<object>((p) => true, (p) => LoadOrdered());
            btnRepair = new RelayCommand<object>((p) => true, (p) => LoadRepair());
            choseRoom = new RelayCommand<object>((p) => true, (p) => ViewDetailRoom(p));
            sortRoom = new RelayCommand<object>((p) => true, (p) => SortRoomF());

            LoadDbRoom();
        }
        private string GetIdStaff(UserControl p)
        {
            FrameworkElement window = GetParent(p);
            var w = window as MainWindow;
            return w._EID.Text;
        }
        FrameworkElement GetParent(UserControl p)
        {
            FrameworkElement parent = p;
            while (parent.Parent != null)
                parent = parent.Parent as FrameworkElement;
            return parent;
        }
        private bool FilterByName(object emp)
        {
            if (!string.IsNullOrEmpty(TextToFilter))
            {
                var empDetail = emp as RoomVM;
                return empDetail != null && empDetail.Name.IndexOf(TextToFilter, StringComparison.OrdinalIgnoreCase) >= 0;
            }
            return true;
        }
        private void SortRoomF()
        {
            if ((string)SortRoom.Content == "Tầng")
            {
                sortFloordb();
                sortFloor();
            }
            if ((string)SortRoom.Content == "Loại phòng")
            {
                _roomListdb = new ObservableCollection<RoomVM>(_roomListdb.OrderBy(i => i.Description));
                RoomList = new ObservableCollection<RoomVM>(RoomList.OrderBy(i => i.Description));
            }
            RoomCollection = CollectionViewSource.GetDefaultView(RoomList);
            RoomCollection.Filter = FilterByName;
        }
        public void sortFloordb()
        {
            var list = new List<RoomVM>(_roomListdb);
            _roomListdb.Clear();
            list.Sort((x, y) => compareFloor(x, y));
            _roomListdb = new ObservableCollection<RoomVM>(list);
            list.Clear();
        }
        public void sortFloor()
        {
            var list = new List<RoomVM>(RoomList);
            RoomList.Clear();
            list.Sort((x, y) => compareFloor(x, y));
            RoomList = new ObservableCollection<RoomVM>(list);
            list.Clear();
        }
        public int compareFloor(RoomVM x, RoomVM y)
        {
            if (x.Name.Substring(1, 1) == y.Name.Substring(1, 1))
            {
                if (x.Name.Substring(0, 1) == y.Name.Substring(0, 1))
                    return x.Name.CompareTo(y.Name);
                return x.Name.Substring(0, 1).CompareTo(y.Name.Substring(0, 1));
            }
            return x.Name.Substring(1, 1).CompareTo(y.Name.Substring(1, 1));
        }
        public void LoadDbRoom()
        {
            var TimeNow = DateTime.Now;
            _roomListdb = new ObservableCollection<RoomVM>();
            RoomList.Clear();
            using (var db = new QLYHOTELEntities())
            {
                var select = from s in db.PHONGs select s;
                foreach (var room in select)
                {
                    string StatusRoom = "Trống";
                    int iDBook = 0;
                    foreach (var info in room.DATs)
                    {
                        if (info.TRANGTHAI == "Đã thanh toán") continue;
                        if ((info.NGAYDAT.Value - TimeNow).TotalMinutes <= 20 && (info.NGAYTRA.Value - TimeNow).TotalMilliseconds > 0)
                        {
                            StatusRoom = info.TRANGTHAI;
                            iDBook = info.MADAT;
                        }
                        if (StatusRoom == "Đang sử dụng")
                            break;
                    }
                    _roomListdb.Add(new RoomVM() { ID = room.MAPHONG, Name = room.TENPHONG.ToString(), Description = room.LOAIPHONG.ToString(), Status = StatusRoom, IDBook = iDBook });
                    RoomList.Add(new RoomVM() { ID = room.MAPHONG, Name = room.TENPHONG.ToString(), Description = room.LOAIPHONG.ToString(), Status = StatusRoom, IDBook = iDBook });
                }
            }
            sortFloordb();
            sortFloor();
            RoomCollection = CollectionViewSource.GetDefaultView(RoomList);
        }
        public void LoadAllRoom()
        {
            RoomList.Clear();
            foreach (var room in _roomListdb)
                RoomList.Add(room);
        }
        public void LoadAvailabel()
        {
            RoomList.Clear();
            foreach (var room in _roomListdb)
                if (room.Status == "Trống")
                    RoomList.Add(room);
        }
        public void LoadOrdered()
        {
            RoomList.Clear();
            foreach (var room in _roomListdb)
                if (room.Status == "Đã đặt")
                    RoomList.Add(room);
        }
        public void LoadRepair()
        {
            RoomList.Clear();
            foreach (var room in _roomListdb)
                if (room.Status == "Tu sửa")
                    RoomList.Add(room);
        }
        private void ViewDetailRoom(object p)
        {
            var room = (RoomVM)p;
            RoomDetail r = new RoomDetail();
            if (room.Status != "Trống")
            {
                using (var db = new QLYHOTELEntities())
                {
                    var select = (from i in db.DATs where i.MADAT == room.IDBook select i).Single();
                    r.idbook.Text = room.IDBook.ToString();
                    r.txblTenKH.Text = select.KHACH.TENKH;
                    r.txblCCCD.Text = select.KHACH.CCCD;
                    r.txblNgayDen.Text = select.NGAYDAT.Value.ToString();
                    r.txbNgayTra.Text = select.NGAYTRA.Value.ToString();
                    r.txblSoNguoi.Text = select.SONG.ToString();
                    r.Uid = idnv.ToString();
                    if (select.TRANGTHAI == "Đã đặt")
                        r.btnAccept.Content = "Nhận phòng";
                    else
                        r.btnAccept.Content = "Thanh toán";
                }
            }
            else
            {
                r.btnAccept.Visibility = Visibility.Collapsed;
                r.btnBookServiece.Visibility = Visibility.Collapsed;
            }
            r.ShowDialog();
        }
    }
}
