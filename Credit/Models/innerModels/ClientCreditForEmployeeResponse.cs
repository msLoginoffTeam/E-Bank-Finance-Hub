﻿using System.ComponentModel.DataAnnotations;

namespace CreditService_Patterns.Models.innerModels;

public class ClientCreditForEmployeeResponse
{
    public required Guid Id { get; set; }
    public required Guid ClientId { get; set; }
    public required Guid AccountId { get; set; }
    public required CreditPlanResponse CreditPlan { get; set; }
    public required float Amount { get; set; }
    public required DateTime ClosingDate { get; set; }
    public required float RemainingAmount { get; set; }
    public required ClientCreditStatusEnum Status { get; set; }
}