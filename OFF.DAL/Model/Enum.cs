using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace OFF.DAL.Model
{
    public enum SubscriptionType
    {
        [EnumMember(Value ="Free Trial")]
        FreeTrial,
        [EnumMember(Value = "OneMonth")]
        OneMonth,
    }

    public enum PaymentStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,
        [EnumMember(Value = "Successful")]
        Successful,
        [EnumMember(Value = "Failed")]
        Failed,
    }
    public enum SubStatus
    {
        [EnumMember(Value = "Not Paid")]
        NotPaid,
        [EnumMember(Value = "Paid")]
        Paid
    }

    public enum subscriptionStatus
    {
        [EnumMember(Value = "Active")]
        Active,
        [EnumMember(Value = "Expired")]
        Expired,
    }
}
