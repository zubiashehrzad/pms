//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PMS.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserDocument
    {
        public int Id { get; set; }
        public int User_Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public byte[] FileInfo { get; set; }
        public Nullable<int> UploadedBy { get; set; }
        public Nullable<System.DateTime> UploadOn { get; set; }
    
        public virtual User User { get; set; }
    }
}
