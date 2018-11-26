Как сделать некорректные состояния невыразимыми на C#
---

Как правило статьи, рассказывающие о проектировании типами, содержат примеры на функциональных языках - Haskell, F# и других. Может показаться, что эта концепция неприменима к объектно-ориентированным языкам, но это не так.

В этой статье я переведу примеры из статьи <abbr title="Scott Wlaschin">Скотта Власчина</abbr> [Проектирование типами: Как сделать некорректные состояния невыразимыми](https://habr.com/post/424895/) на [идеоматичесий](http://blog.ploeh.dk/2015/08/03/idiomatic-or-idiosyncratic/) C#. Также я постараюсь показать, что этот подход применим не только в качестве эксперимента, но и в рабочем коде.

<cut/>

# Создаём доменные типы

Сначала надо портировать типы из [предыдущей статьи серии](https://fsharpforfunandprofit.com/posts/designing-with-types-single-case-dus/), которые используются в примерах на F#.

## Оборачиваем примитивные типы в доменные

Примеры на F# используют доменные типы вместо примитивов для адреса электронной почты, почтового кода США и кода штата. Попробуем сделать обёртку примитивного типа на C#:

```csharp
public sealed class EmailAddress
{
    public string Value { get; }

    public EmailAddress (string value)
    {
        if (value == null)
        {
            throw new ArgumentNullException (nameof (value));
        }
        if (!Regex.IsMatch(value, @"^\S+@\S+\.\S+$"))
        {
            throw new ArgumentException ("Email address must contain an @ sign");
        }

        Value = value;
    }

    public override string ToString ()
        => Value;

    public override bool Equals (object other)
        => other is EmailAddress otherEmailAddress &&
           Value.Equals (otherEmailAddress.Value);

    public override int GetHashCode ()
        => Value.GetHashCode ();

    public static implicit operator string (EmailAddress address)
        => address?.Value;
}
```

```csharp
var a = new EmailAddress ("a@example.com");
var b = new EmailAddress ("b@example.com");

var receiverList = String.Join (";", a, b);
```

Я перенёс проверку корректности адреса из фабричной функции в конструктор, поскольку такая реализация более типична для C#. Также пришлось реализовать сравнение и преобразование к строке, что на F# сделал бы компилятор.

С одной стороны, реализация выглядит довольно объёмной. С другой стороны, специфика адреса электронной почты выражена здесь только проверками в конструкторе и, возможно, логикой сравнения. Большую часть здесь занимает инфраструктурный код, который, к тому же, вряд ли будет меняться. Значит, можно либо [сделать](https://docs.microsoft.com/ru-ru/visualstudio/ide/walkthrough-creating-a-code-snippet?view=vs-2017) [шаблон](https://www.jetbrains.com/resharper/features/code_templates.html), либо, на худой конец, копировать общий код из класса в класс.

Надо отметить, что, создание доменных типов из примитивных значений - это не специфика функционального программирования. Наоборот, использование примитивных типов считается признаком [плохого кода в ООП](https://sourcemaking.com/refactoring/smells/primitive-obsession). Примеры таких обёрток можно у видеть, например, [в NLog](https://github.com/NLog/NLog/blob/v4.5.11/src/NLog/LogLevel.cs) и [в NBitcoin](https://github.com/MetacoSA/NBitcoin/blob/v4.1.1.68/NBitcoin/Money.cs#L268), да и стандартный тип TimeSpan - это, по сути обёртка над числом тиков.

## Создаём объекты-значения

Теперь надо создать аналог [записи](https://docs.microsoft.com/ru-ru/dotnet/fsharp/language-reference/records):

```csharp
public sealed class EmailContactInfo
{
    public EmailAddress EmailAddress { get; }

    public bool IsEmailVerified { get; }

    public EmailContactInfo (EmailAddress emailAddress, bool isEmailVerified)
    {
        if (emailAddress == null)
        {
            throw new ArgumentNullException (nameof (emailAddress));
        }

        EmailAddress = emailAddress;
        IsEmailVerified = isEmailVerified;
    }

    public override string ToString ()
        => $"{EmailAddress}, {(IsEmailVerified ? "verified" : "not verified")}";
}
```

Снова потребовалось больше кода, чем на F#, но большую часть работы можно выполнить за счёт [рефакторингов в IDE](https://docs.microsoft.com/ru-ru/visualstudio/ide/reference/generate-constructor?view=vs-2017).

Как и `EmailAddress`, `EmailContactInfo` - это [объект-значение](https://ru.wikipedia.org/wiki/%D0%9F%D1%80%D0%BE%D0%B1%D0%BB%D0%B5%D0%BC%D0%BD%D0%BE-%D0%BE%D1%80%D0%B8%D0%B5%D0%BD%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%BD%D0%BE%D0%B5_%D0%BF%D1%80%D0%BE%D0%B5%D0%BA%D1%82%D0%B8%D1%80%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5#%D0%9E%D0%B1%D1%8A%D0%B5%D0%BA%D1%82-%D0%B7%D0%BD%D0%B0%D1%87%D0%B5%D0%BD%D0%B8%D0%B5) (в смысле DDD, а не [типы-значения в .NET](https://docs.microsoft.com/ru-ru/dotnet/csharp/language-reference/keywords/value-types)), давно известный и применяемый в объектом моделировании.

Остальные типы - `StateCode`, `ZipCode`, `PostalAddress` и `PersonalName` портируются на C# схожим образом.

# Создаём контакт

Итак, код должен выражать правило "Контакт должен содержать адрес электронной почты или почтовый адрес (или оба адреса)". Требуется выразить это правило таким образом, чтобы корректность состояния была видна из определения типов и проверялась компилятором.

## Выражаем различные состояния контакта

Значит, контакт - это объект, содержащий имя человека и либо адрес электронной почты, либо почтовый адрес, либо оба адреса. Очевидно, один класс не может содержать трёх разных наборов свойств, следовательно, надо определить три разных класса. Все три класса должны содержать имя контакта и при этом должна быть возможность обрабатывать контакты разных типов единообразно, не зная, какие именно адреса содержит контакт. Следовательно, контакт будет представлен абстрактным базовым классом, содержащим имя контакта, и тремя реализациями с различным набором полей.

```csharp
public abstract class Contact
{
    public PersonalName Name { get; }

    protected Contact (PersonalName name)
    {
        if (name == null)
        {
            throw new ArgumentNullException (nameof (name));
        }

        Name = name;
    }
}

public sealed class PostOnlyContact : Contact
{
    private readonly PostalContactInfo post_;

    public PostOnlyContact (PersonalName name, PostalContactInfo post)
        : base (name)
    {
        if (post == null)
        {
            throw new ArgumentNullException (nameof (post));
        }

        post_ = post;
    }
}

public sealed class EmailOnlyContact : Contact
{
    private readonly EmailContactInfo email_;

    public EmailOnlyContact(PersonalName name, EmailContactInfo email)
        : base (name)
    {
        if (email == null)
        {
            throw new ArgumentNullException (nameof (email));
        }

        email_ = email;
    }
}

public sealed class EmailAndPostContact : Contact
{
    private readonly EmailContactInfo email_;

    private readonly PostalContactInfo post_;

    public EmailAndPostContact (PersonalName name, EmailContactInfo email, PostalContactInfo post)
        : base (name)
    {
        if (email == null)
        {
            throw new ArgumentNullException (nameof (email));
        }
        if (post == null)
        {
            throw new ArgumentNullException (nameof (post));
        }

        email_ = email;
        post_ = post;
    }
}
```

Вы можете возразить, что [надо использовать композицию](https://en.wikipedia.org/wiki/Composition_over_inheritance), а не наследование, и вообще надо наследовать поведение, а не данные. Замечания справедливые, но, на мой взгляд, применение иерархии классов здесь оправдано. Во-первых, подклассы не просто представляют особые случаи базового класса, вся иерархия представляет собой одну концепцию - контакт. Три реализации контакта очень точно отражают три случая, оговоренные бизнес-правилом. Во-вторых, взаимосвязь базового класса и его наследников, разделение обязанностей между ними легко прослеживается. В-третьих, если иерархия станет действительно проблемой, можно выделить состояние контакта в отдельную иерархию, как это было сделано в исходном примере. На F# наследование записей невозможно, зато новые типы объявляются достаточно просто, поэтому разбиение было выполнено сразу. На C# же более естественным решением будет разместить поля Name в базовом классе.

## Создание контакта

Создание контакта происходит довольно просто.

```csharp
public abstract class Contact
{
    public static Contact FromEmail (PersonalName name, string emailStr)
    {
        var email = new EmailAddress (emailStr);
        var emailContactInfo = new EmailContactInfo (email, false);
        return new EmailOnlyContact (name, emailContactInfo);
    }
}

var name = new PersonalName ("A", null, "Smith");
var contact = Contact.FromEmail (name, "abc@example.com");
```

Если адрес электронной почты окажется некорректным, этот код выбросит исключение, что можно считать аналогом возврата `None` в исходном примере.

## Обновление контакта

Обновление контакта тоже не вызывает сложностей - надо просто добавить абстрактный метод в тип `Contact`.

```csharp
public abstract class Contact
{
    public abstract Contact UpdatePostalAddress (PostalContactInfo newPostalAddress);
}

public sealed class EmailOnlyContact : Contact
{
    public override Contact UpdatePostalAddress (PostalContactInfo newPostalAddress)
        => new EmailAndPostContact (Name, email_, newPostalAddress);
}

public sealed class PostOnlyContact : Contact
{
    public override Contact UpdatePostalAddress (PostalContactInfo newPostalAddress)
        => new PostOnlyContact (Name, newPostalAddress);
}

public sealed class EmailAndPostContact : Contact
{
    public override Contact UpdatePostalAddress (PostalContactInfo newPostalAddress)
        => new EmailAndPostContact (Name, email_, newPostalAddress);
}

var state = new StateCode ("CA");
var zip = new ZipCode ("97210");
var newPostalAddress = new PostalAddress ("123 Main", "", "Beverly Hills", state, zip);
var newPostalContactInfo = new PostalContactInfo (newPostalAddress, false);
var newContact = contact.UpdatePostalAddress (newPostalContactInfo);
```

Как и при использовании option.Value в F#, здесь возможен выброс исключения из конструкторов, если адрес электронной почты, почтовый индекс или штат указаны неверно, но для C# это является распространённой практикой. Конечно же, в рабочем коде здесь или где-то в вызывающем коде должна быть предусмотрена обработка исключений.

## Обработка контактов вне иерархии

Логично расположить логику обновления контакта в самой иерархии `Contact`. Но что, если требуется выполнить что-то, что не укладывается в её область ответственности? Предположим, что надо отобразить контакты на пользовательском интерфейсе.

Можно, конечно, опять добавить абстрактный метод в базовый класс и продолжать добавлять по новому метод каждый раз, когда понадобится ещё как-то обрабатывать контакты. Но тогда будет нарушен [принцип единственной ответственности](https://ru.wikipedia.org/wiki/%D0%9F%D1%80%D0%B8%D0%BD%D1%86%D0%B8%D0%BF_%D0%B5%D0%B4%D0%B8%D0%BD%D1%81%D1%82%D0%B2%D0%B5%D0%BD%D0%BD%D0%BE%D0%B9_%D0%BE%D1%82%D0%B2%D0%B5%D1%82%D1%81%D1%82%D0%B2%D0%B5%D0%BD%D0%BD%D0%BE%D1%81%D1%82%D0%B8), иерархия `Contact` будет захламлена, а логика обработки размазана между реализациями `Contact` и местами ответственными за, собственно, обработку контактов. В F# такой проблемы не было, хотелось бы, чтобы код на C# был не хуже!

Ближайшим аналогом сопоставления с образцом в C# является конструкция switch. Можно было бы добавить в `Contact` свойство перечислимого типа, которое позволяло бы определить реальный тип контакта и выполнить преобразование. Также можно было бы использовать более новые возможности C# и выполнять switch по типу экземпляра `Contact`. Но ведь мы хотели, чтобы при добавлении новых корректных состояний `Contact` компилятор сам подсказывал, где не хватает обработки новых случаев, а switch не гарантирует обработку всех возможных случаев.

Решение - шаблон Посетитель (Visitor). Он позволяет выбирать обработчик в зависимости от реализации `Contact`, отвязывает методы обработки контактов от их иерархии, и, если добавится новый тип контакта, и, соответственно, новый метод в интерфейсе Посетителя, то потребуется его написать во всех реализациях интерфейса. Все требования выполнены!

```csharp
public abstract class Contact
{
    public abstract void AcceptVisitor (IContactVisitor visitor);
}

public interface IContactVisitor
{
    void Visit (PersonalName name, EmailContactInfo email);

    void Visit (PersonalName name, PostalContactInfo post);

    void Visit (PersonalName name, EmailContactInfo email, PostalContactInfo post);
}

public sealed class EmailOnlyContact : Contact
{
    public override void AcceptVisitor (IContactVisitor visitor)
    {
        if (visitor == null)
        {
            throw new ArgumentNullException (nameof (visitor));
        }

        visitor.Visit (Name, email_);
    }
}

public sealed class PostOnlyContact : Contact
{
    public override void AcceptVisitor (IContactVisitor visitor)
    {
        if (visitor == null)
        {
            throw new ArgumentNullException (nameof (visitor));
        }

        visitor.Visit (Name, post_);
    }
}

public sealed class EmailAndPostContact : Contact
{
    public override void AcceptVisitor (IContactVisitor visitor)
    {
        if (visitor == null)
        {
            throw new ArgumentNullException (nameof (visitor));
        }

        visitor.Visit (Name, email_, post_);
    }
}
```

Теперь можно написать код для отображения контактов. Для простоты я буду использовать консольный интерфейс.

```csharp
public sealed class ContactUi
{
    private sealed class Visitor : IContactVisitor
    {
        void IContactVisitor.Visit (PersonalName name, EmailContactInfo email)
        {
            Console.WriteLine (name);
            Console.WriteLine ("* Email: {0}", email);
        }

        void IContactVisitor.Visit (PersonalName name, PostalContactInfo post)
        {
            Console.WriteLine (name);
            Console.WriteLine ("* Postal address: {0}", post);
        }

        void IContactVisitor.Visit (PersonalName name, EmailContactInfo email, PostalContactInfo post)
        {
            Console.WriteLine (name);
            Console.WriteLine ("* Email: {0}", email);
            Console.WriteLine ("* Postal address: {0}", post);
        }
    }

    public void Display (Contact contact)
        => contact.AcceptVisitor (new Visitor ());
}

var ui = new ContactUi ();
ui.Display (newContact);
```

## Дальнейшие улучшения

Если `Contact` объявлен в библиотеке и появление новых наследников в клиентах библиотеки нежелательно, то можно изменить область видимости конструктора `Contact` на `internal`, либо вообще сделать его наследников вложенными классами, объявить видимость реализаций и конструктора `private`, а создание экземпляров делать через только статические методы-фабрики.

```csharp
public abstract class Contact
{
    private sealed class EmailOnlyContact : Contact
    {
        public EmailOnlyContact (PersonalName name, EmailContactInfo email)
            : base (name)
        {

        }
    }

    private Contact (PersonalName name)
    {

    }

    public static Contact EmailOnly (PersonalName name, EmailContactInfo email)
        => new EmailOnlyContact (name, email);
}
```

Таким образом можно воспроизвести нерасширяемость типа-суммы, хотя, как правило, этого не требуется.

# Заключение

Надеюсь, мне удалось показать, как средствами ООП ограничить корректные состояния бизнес-логики при помощи типов. Код получился более объёмным, чем на F#. Где-то это обусловлено относительной громоздкостью решений ООП, где-то - многословностью языка, но решения нельзя назвать непрактичными.

Что интересно, начав с чисто функционального решения, мы пришли к следованию рекомендация предметно-ориентированного программирования и паттернам ООП. На самом деле, это не удивительно, потому что подобие типов-сумм и паттерна Посетитель [известно](http://blog.ploeh.dk/2018/06/25/visitor-as-a-sum-type/), и довольно [давно](http://www.drdobbs.com/cpp/discriminated-unions-i/184403821). Целью этой статьи было показать не столько конкретный приём, сколько продемонстрировать применимость идей из ["башни из слоновой кости"](https://fsharpforfunandprofit.com/fppatterns/#twitterstorm) в императивном программировании. Конечно, не всё получится перенести так же легко, но с появлением всё новых и новых функциональных возможностей в мейнстримовых языках программирования границы применимого будут расширятся.
