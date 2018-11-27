using System;

namespace Demo
{
    /// <summary>
    /// Представляет имя человека.
    /// </summary>
    /// <remarks>
    /// Первая и последняя часть имени в общем случае не являются личным именем и фамилией.
    /// Корректная трактовка частей имени выходит за рамки данного примера.
    /// </remarks>
    public sealed class PersonalName
    {
        /// <summary>
        /// Возвращает первую часть имени.
        /// </summary>
        /// <value>Первая часть имени.</value>
        public string FirstName { get; }

        /// <summary>
        /// Возвращает инициал в середине имени.
        /// </summary>
        /// <value>
        /// Инициал в середине имени, либо <see langword="null" />, если в середине имени нет
        /// инициала.
        /// </value>
        public string MiddleInitial { get; }

        /// <summary>
        /// Возвращает последнюю часть имени.
        /// </summary>
        /// <value>Последняя часть имени.</value>
        public string LastName { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="PersonalName"/>.
        /// </summary>
        /// <param name="firstName">Первая часть имени.</param>
        /// <param name="middleInitial">
        /// Инициал в середине имени, либо <see langword="null" />, если в середине имени нет
        /// инициала.
        /// </param>
        /// <param name="lastName">Последняя часть имени.</param>
        /// <exception cref="ArgumentNullException">
        /// Параметр <paramref name="firstName"/> имеет значение <see langword="null"/>.
        /// -или-
        /// Параметр <paramref name="lastName"/> имеет значение <see langword="null"/>.
        /// </exception>
        public PersonalName(string firstName, string middleInitial, string lastName)
        {
            if (firstName == null)
            {
                throw new ArgumentNullException(nameof(firstName));
            }
            if (lastName == null)
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            FirstName = firstName;
            MiddleInitial = middleInitial;
            LastName = lastName;
        }

        /// <inheritdoc />
        public override string ToString()
            => MiddleInitial != null
                ? $"{FirstName} {MiddleInitial} {LastName}"
                : $"{FirstName} {LastName}";
    }
}
