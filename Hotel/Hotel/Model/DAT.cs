//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Hotel.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class DAT
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DAT()
        {
            this.CUNGCAPs = new HashSet<CUNGCAP>();
        }
    
        public int MADAT { get; set; }
        public Nullable<int> MANV { get; set; }
        public Nullable<int> MAKH { get; set; }
        public Nullable<int> MAPHONG { get; set; }
        public Nullable<int> SONG { get; set; }
        public Nullable<System.DateTime> NGAYDAT { get; set; }
        public Nullable<System.DateTime> NGAYTRA { get; set; }
        public Nullable<bool> THANHTOAN { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CUNGCAP> CUNGCAPs { get; set; }
        public virtual KHACH KHACH { get; set; }
        public virtual NHANVIEN NHANVIEN { get; set; }
        public virtual PHONG PHONG { get; set; }
    }
}
