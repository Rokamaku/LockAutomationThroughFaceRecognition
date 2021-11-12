using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Para.LockAutomation.DTO
{
    public class FilterModel
    {
        public const int MaxPageSize = 100;
        public const int DefaultPageSize = 10;
        private int _pageSize;

        [Required]
        public int PageSize
        {
            get
            {
                //ensure the page size isn't larger than the maximum.
                if (_pageSize == 0 || _pageSize > MaxPageSize)
                {
                    return DefaultPageSize;
                }
                return _pageSize;
            }
            set
            {
                _pageSize = value;
            }
        }

        [Required]
        private int _pageNumber;
        public int PageNumber
        {
            get { return _pageNumber < 1 ? 1 : _pageNumber; }
            set { _pageNumber = value; }
        }

        //public string SortField { get; set; }

        [Required]
        public string SortType { get; set; }

        //public string SearchField { get; set; }

        //public string SearchString { get; set; }

        //public string FilterField { get; set; }

        //public string FilterValue { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public FilterModel()
        {
            SortType = "asc";
            PageNumber = 1;
        }
    }
}
