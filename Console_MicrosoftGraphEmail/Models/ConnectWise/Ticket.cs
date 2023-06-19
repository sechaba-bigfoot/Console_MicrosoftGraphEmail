using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Console_MicrosoftGraphEmail.Models.ConnectWise
{
    public class Ticket
    {
        public int id { get; set; }
        public string summary { get; set; }
        public string recordType { get; set; }
        public Board board { get; set; }
        public Status status { get; set; }
        public Workrole workRole { get; set; }
        public Worktype workType { get; set; }
        public Company company { get; set; }
        public Site site { get; set; }
        public string siteName { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string stateIdentifier { get; set; }
        public string zip { get; set; }
        public Country country { get; set; }
        public Contact contact { get; set; }
        public string contactName { get; set; }
        public string contactPhoneNumber { get; set; }
        public string contactPhoneExtension { get; set; }
        public string contactEmailAddress { get; set; }
        public Type type { get; set; }
        public Subtype subType { get; set; }
        public Item item { get; set; }
        public Team team { get; set; }
        public Owner owner { get; set; }
        public Priority priority { get; set; }
        public Servicelocation serviceLocation { get; set; }
        public Source source { get; set; }
        public DateTime requiredDate { get; set; }
        //public  budgetHours { get; set; }
        public Opportunity opportunity { get; set; }
        public Agreement agreement { get; set; }
        public string severity { get; set; }
        public string impact { get; set; }
        public string externalXRef { get; set; }
        public string poNumber { get; set; }
        public int knowledgeBaseCategoryId { get; set; }
        public int knowledgeBaseSubCategoryId { get; set; }
        public bool allowAllClientsPortalView { get; set; }
        public bool customerUpdatedFlag { get; set; }
        public bool automaticEmailContactFlag { get; set; }
        public bool automaticEmailResourceFlag { get; set; }
        public bool automaticEmailCcFlag { get; set; }
        public string automaticEmailCc { get; set; }
        public string initialDescription { get; set; }
        public string initialInternalAnalysis { get; set; }
        public string initialResolution { get; set; }
        public string initialDescriptionFrom { get; set; }
        public string contactEmailLookup { get; set; }
        public bool processNotifications { get; set; }
        public bool skipCallback { get; set; }
        public string closedDate { get; set; }
        public string closedBy { get; set; }
        public bool closedFlag { get; set; }
        //public int actualHours { get; set; }
        //public bool approved { get; set; }
        //public int estimatedExpenseCost { get; set; }
        //public int estimatedExpenseRevenue { get; set; }
        //public int estimatedProductCost { get; set; }
        //public int estimatedProductRevenue { get; set; }
        //public int estimatedTimeCost { get; set; }
        //public int estimatedTimeRevenue { get; set; }
        //public string billingMethod { get; set; }
        //public int billingAmount { get; set; }
        //public int hourlyRate { get; set; }
        //public string subBillingMethod { get; set; }
        //public int subBillingAmount { get; set; }
        //public string subDateAccepted { get; set; }
        //public string dateResolved { get; set; }
        //public string dateResplan { get; set; }
        //public string dateResponded { get; set; }
        //public int resolveMinutes { get; set; }
        //public int resPlanMinutes { get; set; }
        //public int respondMinutes { get; set; }
        //public bool isInSla { get; set; }
        //public int knowledgeBaseLinkId { get; set; }
        //public string resources { get; set; }
        //public int parentTicketId { get; set; }
        //public bool hasChildTicket { get; set; }
        //public bool hasMergedChildTicketFlag { get; set; }
        //public string knowledgeBaseLinkType { get; set; }
        //public string billTime { get; set; }
        //public string billExpenses { get; set; }
        //public string billProducts { get; set; }
        //public string predecessorType { get; set; }
        //public int predecessorId { get; set; }
        //public bool predecessorClosedFlag { get; set; }
        //public int lagDays { get; set; }
        //public bool lagNonworkingDaysFlag { get; set; }
        //public DateTime estimatedStartDate { get; set; }
        //public int duration { get; set; }
        //public Location location { get; set; }
        public Department department { get; set; }
        //public string mobileGuid { get; set; }
        //public Sla sla { get; set; }
        //public string slaStatus { get; set; }
        //public Currency currency { get; set; }
        //public Mergedparentticket mergedParentTicket { get; set; }
        //public string[] integratorTags { get; set; }
        //public _Info23 _info { get; set; }
        //public Customfield[] customFields { get; set; }
    }

    public class Board
    {
        public int id { get; set; }
        public string name { get; set; }
        public _Info _info { get; set; }
    }

    public class _Info
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Status
    {
        public int id { get; set; }
        public string name { get; set; }
        public _Info1 _info { get; set; }
    }

    public class _Info1
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Workrole
    {
        public int id { get; set; }
        public string name { get; set; }
        public _Info2 _info { get; set; }
    }

    public class _Info2
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Worktype
    {
        public int id { get; set; }
        public string name { get; set; }
        public _Info3 _info { get; set; }
    }

    public class _Info3
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Company
    {
        public int id { get; set; }
        public string identifier { get; set; }
        public string name { get; set; }
        public _Info4 _info { get; set; }
    }

    public class _Info4
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Site
    {
        public int id { get; set; }
        public string name { get; set; }
        public _Info5 _info { get; set; }
    }

    public class _Info5
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Country
    {
        public int id { get; set; }
        public string identifier { get; set; }
        public string name { get; set; }
        public _Info6 _info { get; set; }
    }

    public class _Info6
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Contact
    {
        public int id { get; set; }
        public string name { get; set; }
        public _Info7 _info { get; set; }
    }

    public class _Info7
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Type
    {
        public int id { get; set; }
        public string name { get; set; }
        public _Info8 _info { get; set; }
    }

    public class _Info8
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Subtype
    {
        public int id { get; set; }
        public string name { get; set; }
        public _Info9 _info { get; set; }
    }

    public class _Info9
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Item
    {
        public int id { get; set; }
        public string name { get; set; }
        public _Info10 _info { get; set; }
    }

    public class _Info10
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Team
    {
        public int id { get; set; }
        public string name { get; set; }
        public _Info11 _info { get; set; }
    }

    public class _Info11
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Owner
    {
        public int id { get; set; }
        public string identifier { get; set; }
        public string name { get; set; }
        public _Info12 _info { get; set; }
    }

    public class _Info12
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Priority
    {
        public int id { get; set; }
        public string name { get; set; }
        public int sort { get; set; }
        public _Info13 _info { get; set; }
    }

    public class _Info13
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Servicelocation
    {
        public int id { get; set; }
        public string name { get; set; }
        public _Info14 _info { get; set; }
    }

    public class _Info14
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Source
    {
        public int id { get; set; }
        public string name { get; set; }
        public _Info15 _info { get; set; }
    }

    public class _Info15
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Opportunity
    {
        public int id { get; set; }
        public string name { get; set; }
        public _Info16 _info { get; set; }
    }

    public class _Info16
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Agreement
    {
        public int id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public _Info17 _info { get; set; }
    }

    public class _Info17
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Location
    {
        public int id { get; set; }
        public string name { get; set; }
        public _Info18 _info { get; set; }
    }

    public class _Info18
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Department
    {
        public int id { get; set; }
        public string identifier { get; set; }
        public string name { get; set; }
        public _Info19 _info { get; set; }
    }

    public class _Info19
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Sla
    {
        public int id { get; set; }
        public string name { get; set; }
        public _Info20 _info { get; set; }
    }

    public class _Info20
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Currency
    {
        public int id { get; set; }
        public string symbol { get; set; }
        public string currencyCode { get; set; }
        public string decimalSeparator { get; set; }
        public int numberOfDecimals { get; set; }
        public string thousandsSeparator { get; set; }
        public bool negativeParenthesesFlag { get; set; }
        public bool displaySymbolFlag { get; set; }
        public string currencyIdentifier { get; set; }
        public bool displayIdFlag { get; set; }
        public bool rightAlign { get; set; }
        public string name { get; set; }
        public _Info21 _info { get; set; }
    }

    public class _Info21
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Mergedparentticket
    {
        public int id { get; set; }
        public string summary { get; set; }
        public _Info22 _info { get; set; }
    }

    public class _Info22
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class _Info23
    {
        public string additionalProp1 { get; set; }
        public string additionalProp2 { get; set; }
        public string additionalProp3 { get; set; }
    }

    public class Customfield
    {
        public int id { get; set; }
        public string caption { get; set; }
        public string type { get; set; }
        public string entryMethod { get; set; }
        public int numberOfDecimals { get; set; }
        public Value value { get; set; }
    }

    public class Value
    {
    }

}
