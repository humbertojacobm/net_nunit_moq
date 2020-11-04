using System;
using System.Collections.Generic;
using System.Text;

namespace Loans.Domain.Applications
{
    public interface INowProvider
    {
        DateTime GetNow();
    }
}
