namespace TicketSysteemMVC5.Config
{
    public static class RoleNames
    {
        public const string Administrator = "Administrator";

        public const string Medewerker = "Medewerker";

        public const string Klant = "Klant";

        public const string CreeerTickets = "Administrator,Klant";

        public const string BewerkTickets = "Administrator,Medewerker,Klant";

        public const string BewerkApplicaties = "Administrator, Medewerker";
    }
}