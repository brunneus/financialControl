using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinanceControl.Application.Expenses
{
    public record ExpenseRequest(decimal Value, DateTime Date, ExpenseType Type, bool Recurrent);
}