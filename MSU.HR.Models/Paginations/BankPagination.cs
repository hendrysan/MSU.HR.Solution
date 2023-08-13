﻿using MSU.HR.Models.Entities;
using MSU.HR.Models.Others;

namespace MSU.HR.Models.Paginations
{
    public class BankPagination
    {
        public List<Bank> Banks { get; set; }
        public PaginationModel Pagination { get; set; }
    }
}
