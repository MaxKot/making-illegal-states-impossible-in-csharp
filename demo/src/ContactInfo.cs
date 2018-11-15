
namespace Demo
{
    /// <summary>Представляет контактные данные.</summary>
    public abstract class ContactInfo
    {
        /// <summary>
        /// Производит двойную диспетчеризацию в зависимости от реализации
        /// <see cref="ContactInfo"/>.
        /// </summary>
        /// <param name="visitor">
        /// Объект, обрабатывающий вызовы конкретных реализации <see cref="ContactInfo"/>.
        /// </param>
        public abstract void AcceptVisitor (IContactInfoVisitor visitor);
    }
}
