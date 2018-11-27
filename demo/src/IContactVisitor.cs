
namespace Demo
{
    /// <summary>
    /// Предоставляет методы обработки <see cref="Contact"/>, выбираемые в зависимости от реального
    /// типа объекта.
    /// </summary>
    public interface IContactVisitor
    {
        /// <summary>
        /// Обрабатывает контактные данные, содержащие только адрес электронной почты.
        /// </summary>
        /// <param name="name">Имя человека.</param>
        /// <param name="email">Адрес электронной почты.</param>
        void Visit(PersonalName name, EmailContactInfo email);

        /// <summary>
        /// Обрабатывает контактные данные, содержащие только почтовый адрес.
        /// </summary>
        /// <param name="name">Имя человека.</param>
        /// <param name="post">Почтовый адрес.</param>
        void Visit(PersonalName name, PostalContactInfo post);

        /// <summary>
        /// Обрабатывает контактные данные, содержащие и адрес электронной почты, и почтовый адрес.
        /// </summary>
        /// <param name="name">Имя человека.</param>
        /// <param name="email">Адрес электронной почты.</param>
        /// <param name="post">Почтовый адрес.</param>
        void Visit(PersonalName name, EmailContactInfo email, PostalContactInfo post);
    }
}
