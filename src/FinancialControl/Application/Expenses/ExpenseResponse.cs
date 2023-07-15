using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FinanceControl.Domain.Expenses;

namespace FinanceControl.Application.Expenses
{
    public class ExpenseResponse
    {
        public Guid Id { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }

        public static implicit operator ExpenseResponse(Expense expense) => new ExpenseResponse
        {
            Id = expense.Id,
            Value = expense.Value,
            Date = expense.Date,
        };
    }
}