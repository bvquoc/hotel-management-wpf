﻿using Hotel.Model;
using Hotel.View;
using MahApps.Metro.Behaviors;
using Microsoft.Xaml.Behaviors.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Hotel.ViewModel
{
    internal class ReservationBookViewModel : BaseViewModel
    {
        private static ObservableCollection<RoomVM> _rooms;
        public ObservableCollection<RoomVM> Rooms
        {
            get { return _rooms; }
            set { _rooms = value; OnPropertyChanged(); }
        }
        private ObservableCollection<RoomVM> _selectedRooms;
        public ObservableCollection<RoomVM> SelectedRooms
        {
            get { return _selectedRooms; }
            set { _selectedRooms = value; OnPropertyChanged(); }
        }
        private DateTime _dateStart;
        public DateTime DateStart
        {
            get { return _dateStart; }
            set { _dateStart = value; OnPropertyChanged(); LoadRoom(); }
        }
        private DateTime _dateEnd;
        public DateTime DateEnd
        {
            get { return _dateEnd; }
            set { _dateEnd = value; OnPropertyChanged(); LoadRoom(); }
        }
        private DateTime _timeStart;

        public DateTime TimeStart
        {
            get { return _timeStart; }
            set { _timeStart = value; OnPropertyChanged(); LoadRoom(); }
        }
        private DateTime _timeEnd;

        public DateTime TimeEnd
        {
            get { return _timeEnd; }
            set { _timeEnd = value; OnPropertyChanged(); LoadRoom(); }
        }
        private int _idCus;
        private string _cccd;

        public string CCCD
        {
            get { return _cccd; }
            set { _cccd = value; OnPropertyChanged(); AutoCCCD(); }
        }
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged(); }
        }
        private string _sdt;

        public string SDT
        {
            get { return _sdt; }
            set { _sdt = value; OnPropertyChanged(); }
        }
        private string _diachi;

        public string DiaChi
        {
            get { return _diachi; }
            set { _diachi = value; OnPropertyChanged(); }
        }
        private int _sex;

        public int Sex
        {
            get { return _sex; }
            set { _sex = value; OnPropertyChanged(); }
        }


        public ICommand ChoseRoom { get; set; }
        public ICommand DeleteSelected { get; set; }
        public ICommand CancelReservate { get; set; }
        public ICommand SaveReservate { get; set; }
        public ReservationBookViewModel()
        {
            ChoseRoom = new RelayCommand<object>((p) => true, (p) => addRoom(p));
            DeleteSelected = new RelayCommand<object>((p) => true, (p) => deleteSelected(p));
            CancelReservate = new RelayCommand<ReservationBookView>((p) => true, (p) => Reload(p));
            SaveReservate = new RelayCommand<ReservationBookView>((p) => true, (p) => Save(p));
            Rooms = new ObservableCollection<RoomVM>();
            SelectedRooms = new ObservableCollection<RoomVM>();
            DateStart = DateTime.Now;
            DateEnd = DateTime.Now;
            TimeStart = DateTime.Now;
            TimeEnd = DateTime.Now;
            clearInfo();
            LoadRoom();
        }
        private void clearInfo()
        {
            CCCD = "";
            Name = "";
            SDT = "";
            DiaChi = "";
            Sex = -1;
        }
        private void LoadRoom()
        {
            DateTime start = new DateTime(DateStart.Year, DateStart.Month, DateStart.Day, TimeStart.Hour, TimeStart.Minute, TimeStart.Second);
            DateTime end = new DateTime(DateEnd.Year, DateEnd.Month, DateEnd.Day, TimeEnd.Hour, TimeEnd.Minute, TimeEnd.Second);

            Rooms.Clear();
            if (DateTime.Compare(start, end) >= 0 ||
                DateTime.Compare(start, DateTime.Now) < 0)
                return;
            using (var db = new QLYHOTELEntities())
            {
                var select = from s in db.PHONGs select s;
                foreach (var room in select)
                {
                    bool ok = true;
                    foreach (var dat in room.DATs)
                    {
                        if (dat.TRANGTHAI == "Đã thanh toán") continue;
                        if (DateTime.Compare(dat.NGAYTRA.Value, start) >= 0 &&
                            DateTime.Compare(dat.NGAYDAT.Value, end) <= 0)
                        {
                            ok = false;
                            break;
                        }
                    }

                    if (ok)
                    {
                        foreach (var sele in SelectedRooms)
                        {
                            if (sele.ID != room.MAPHONG) continue;
                            if (DateTime.Compare(sele.DateEnd, start) >= 0 &&
                            DateTime.Compare(sele.DateStart, end) <= 0)
                            {
                                ok = false;
                                break;
                            }
                        }
                    }
                    if (ok)
                        Rooms.Add(new RoomVM() { ID = room.MAPHONG, Name = room.TENPHONG.ToString(), Description = room.LOAIPHONG.ToString(), Status = room.TRANGTHAI.ToString() });
                }
            }
        }
        private void addRoom(object p)
        {
            var SelectedRoom = (RoomVM)p;
            if (SelectedRoom == null) return;
            SelectedRooms.Insert(0, new RoomVM()
            {
                ID = SelectedRoom.ID,
                Name = SelectedRoom.Name,
                Description = SelectedRoom.Description,
                Status = SelectedRoom.Status,
                NumPeo = SelectedRoom.NumPeo,
                DateStart = new DateTime(DateStart.Year, DateStart.Month, DateStart.Day, TimeStart.Hour, TimeStart.Minute, TimeStart.Second),
                DateEnd = new DateTime(DateEnd.Year, DateEnd.Month, DateEnd.Day, TimeEnd.Hour, TimeEnd.Minute, TimeEnd.Second)
            });
            Rooms.Remove(SelectedRoom);
        }
        private void AutoCCCD()
        {
            _idCus = -1;
            if (CCCD.Length == 9 || CCCD.Length == 12)
            {
                using (var db = new QLYHOTELEntities())
                {
                    var select = from s in db.KHACHes select s;
                    foreach (var p in select)
                    {
                        if (p.CCCD == CCCD)
                        {
                            _idCus = p.MAKH;
                            Name = p.TENKH;
                            DiaChi = p.DCHI;
                            SDT = p.SDT;
                            if (p.GIOITINH == "Nam")
                                Sex = 0;
                            else if (p.GIOITINH == "Nữ")
                                Sex = 1;
                            else
                                Sex = 2;
                            return;
                        }
                    }
                }
            }
        }
        private void deleteSelected(object p)
        {
            try
            {
                var room = (RoomVM)p;
                SelectedRooms.Remove(room);
                LoadRoom();
            }
            catch (Exception ex)
            {
                new DialogCustomize(ex.Message).ShowDialog();
            }
        }
        private void Reload(ReservationBookView p)
        {
            p.DataContext = new ReservationBookViewModel();
            clearInfo();
        }
        private void Save(ReservationBookView p)
        {
            if (!CheckInfo()) return;
            updateIdCus();
            //mã nhân viên ?
            using (var db = new QLYHOTELEntities())
            {
                foreach (var room in SelectedRooms)
                {
                    var info = new DAT();
                    info.MAKH = _idCus;
                    info.MANV = 1; // !!!!
                    info.MAPHONG = room.ID;
                    info.SONG = room.NumPeo;
                    info.NGAYDAT = room.DateStart;
                    info.NGAYTRA = room.DateEnd;
                    info.TRANGTHAI = "Đã đặt";
                    db.DATs.Add(info);
                    db.SaveChanges();
                }
            }
            new DialogCustomize("Thành công!").ShowDialog();
            Reload(p);
        }
        private void updateIdCus()
        {
            var cus = new KHACH();
            cus.CCCD = CCCD;
            cus.TENKH = Name;
            cus.SDT = SDT;
            cus.DCHI = DiaChi;
            cus.GIOITINH = "Khác";
            if (Sex == 0) cus.GIOITINH = "Nam";
            if (Sex == 1) cus.GIOITINH = "Nữ";

            if (_idCus == -1)
            {
                using (var db = new QLYHOTELEntities())
                {
                    db.KHACHes.Add(cus);
                    db.SaveChanges();
                    var newcus = (from d in db.KHACHes where d.CCCD == CCCD select d).Single();
                    _idCus = newcus.MAKH;
                }
            }
            else
            {
                using (var db = new QLYHOTELEntities())
                {
                    var update = (from u in db.KHACHes where u.MAKH == _idCus select u).Single();
                    update.TENKH = cus.TENKH;
                    update.CCCD = cus.CCCD;
                    update.SDT = cus.SDT;
                    update.DCHI = cus.DCHI;
                    update.GIOITINH = cus.GIOITINH;
                    db.SaveChanges();
                }
            }
        }
        private bool CheckInfo()
        {
            try
            {
                if (CCCD.Length != 9 && CCCD.Length != 12)
                    throw new Exception("CMND/CCCD không hợp lệ!");
                if (SDT.Length != 10)
                    throw new Exception("Số điện thoại không hợp lệ!");
                if (Name == "")
                    throw new Exception("Tên không hợp lệ!");
                if (DiaChi == "")
                    throw new Exception("Địa chỉ không hợp lệ!");
                if (Sex == -1)
                    throw new Exception("Giới tính không hợp lệ!");
                if (SelectedRooms.Count == 0)
                    throw new Exception("Không có thông tin phòng!");
                foreach (var room in SelectedRooms)
                    if (room.NumPeo <= 0)
                        throw new Exception("Phòng chưa nhập số người!");
            }
            catch (Exception ex)
            {
                new DialogCustomize(ex.Message).ShowDialog();
                return false;
            }
            return true;
        }
    }
}
