using Para.LockAutomation.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Para.LockAutomation.DTO
{
    public class SearchResultDTO<TData, THeader> where THeader : PaginationHeader
    {
        public THeader PaginationHeader { get; set; }
        public IEnumerable<TData> Data { get; set; }

    }

    public class PaginationHeader
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int? TotalCount { get; set; }
        public int? TotalPages { get; set; }
    }

    public class FaceLogSearchResultDTO: SearchResultDTO<FaceLogEntity, PaginationHeader>
    {

    }
}
