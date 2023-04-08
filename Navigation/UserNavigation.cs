namespace gestionDiversidad.Navigation
{
    public class UserNavigation
    {
        public string nif { get; set; } = null!;
        public int rol { get; set; }
        public UserNavigation? padre;
        public UserNavigation(UserNavigation padre)
        {
            this.padre = padre;
        }

        [ActivatorUtilitiesConstructor]
        public UserNavigation(string nif, int rol, UserNavigation? padre = null)
        {
            this.nif = nif;
            this.rol = rol;
            this.padre = padre;
        }

        public UserNavigation()
        {

        }
    }
}
