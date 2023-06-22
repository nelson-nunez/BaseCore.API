using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseRest.Core.Model.Base
{
    public class PagedDataResponse<T> where T : class
    {
        public PagedDataResponse()
        { }
        public PagedDataResponse(int pageIndex, int pageSize, int rowCount, IList<T> result)
        {
            this.PageSize = pageSize == 0 ? rowCount : pageSize;
            this.PageIndex = pageIndex;
            this.PageCount = pageSize > 0 ? Convert.ToInt32(Math.Ceiling((decimal)rowCount / (decimal)pageSize)) : 1;
            this.RowCount = rowCount;
            this.Results = result;
        }

        public int PageIndex { get; set; }

        public int PageCount { get; set; }

        public int PageSize { get; set; }

        public int RowCount { get; set; }

        public IList<T> Results { get; set; }
    }
}