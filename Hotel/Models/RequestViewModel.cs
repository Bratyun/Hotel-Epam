using DAL.EntityClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Hotel.Models
{
    public class RequestViewModel
    {
        [Required]
        public int Id { get; set; }
        [Required]
        [Range(0, 10, ErrorMessage = "Take value between 0 and 10")]
        public int RoomSize { get; set; }
        [Required]
        [Range(0, 5, ErrorMessage = "Take value between 0 and 5")]
        public int Comfort { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd'/'MM'/'yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        public string UserId { get; set; }
        [Required]
        public RequestStatus Status { get; set; }

        public RequestViewModel()
        { }

        public RequestViewModel(Request request)
        {
            Id = request.Id;
            RoomSize = request.RoomSize;
            Comfort = request.Comfort;
            StartDate = request.StartDate;
            EndDate = request.EndDate;
            UserId = request.UserId;
            Status = request.Status;
        }
    }
}