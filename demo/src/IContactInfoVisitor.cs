
namespace Demo
{
    /// <summary>
    /// Предоставляет методы обработки <see cref="ContactInfo"/>, выбираемые в зависимости от
    /// реального типа объекта.
    /// </summary>
    public interface IContactInfoVisitor
    {
        /// <summary>
        /// Обрабатывает контактные данные, содержащие только адрес электронной почты.
        /// </summary>
        /// <param name="email">Адрес электронной почты.</param>
        void Visit (EmailContactInfo email);

        /// <summary>
        /// Обрабатывает контактные данные, содержащие только почтовый адрес.
        /// </summary>
        /// <param name="post">Почтовый адрес.</param>
        void Visit (PostalContactInfo post);

        /// <summary>
        /// Обрабатывает контактные данные, содержащие и адрес электронной почты, и почтовый адрес.
        /// </summary>
        /// <param name="email">Адрес электронной почты.</param>
        /// <param name="post">Почтовый адрес.</param>
        void Visit (EmailContactInfo email, PostalContactInfo post);
    }
}
