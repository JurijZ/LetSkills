using System;
using System.Collections.Generic;

namespace LetSkillsBackend.Models
{
    public partial class Users
    {
        public Users()
        {
            Address = new HashSet<Address>();
            ClientImagesMap = new HashSet<ClientImagesMap>();
            CustomerPayments = new HashSet<CustomerPayments>();
            FeedbackFromClients = new HashSet<FeedbackFromClients>();
            FeedbackFromProviders = new HashSet<FeedbackFromProviders>();
            Jobs = new HashSet<Jobs>();
            OffersClient = new HashSet<Offers>();
            OffersProvider = new HashSet<Offers>();
            PaymentsClient = new HashSet<Payments>();
            PaymentsProvider = new HashSet<Payments>();
            UserClientDetails = new HashSet<UserClientDetails>();
            UserProviderDetails = new HashSet<UserProviderDetails>();
            Wallet = new HashSet<Wallet>();
            WalletHistory = new HashSet<WalletHistory>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public byte[] Password { get; set; }
        public string AddressId { get; set; }
        public bool? IsClient { get; set; }
        public bool? IsProvider { get; set; }
        public DateTime? LastLoginTime { get; set; }

        public ICollection<Address> Address { get; set; }
        public ICollection<ClientImagesMap> ClientImagesMap { get; set; }
        public ICollection<CustomerPayments> CustomerPayments { get; set; }
        public ICollection<FeedbackFromClients> FeedbackFromClients { get; set; }
        public ICollection<FeedbackFromProviders> FeedbackFromProviders { get; set; }
        public ICollection<Jobs> Jobs { get; set; }
        public ICollection<Offers> OffersClient { get; set; }
        public ICollection<Offers> OffersProvider { get; set; }
        public ICollection<Payments> PaymentsClient { get; set; }
        public ICollection<Payments> PaymentsProvider { get; set; }
        public ICollection<UserClientDetails> UserClientDetails { get; set; }
        public ICollection<UserProviderDetails> UserProviderDetails { get; set; }
        public ICollection<Wallet> Wallet { get; set; }
        public ICollection<WalletHistory> WalletHistory { get; set; }
    }
}
