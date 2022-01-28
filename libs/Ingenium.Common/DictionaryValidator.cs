using FluentValidation;

/// <summary>
/// Provides validation of key value pairs.
/// </summary>
/// <typeparam name="TKey">The key type.</typeparam>
/// <typeparam name="TValue">The value type.</typeparam>
/// <typeparam name="TValidator">The value validator.</typeparam>
public class DictionaryValidator<TKey, TValue, TValidator> : AbstractValidator<KeyValuePair<TKey, TValue>>
	where TValidator : IValidator<TValue>
{
	public DictionaryValidator(TValidator validator, bool validateKey = true)
	{
		Ensure.IsNotNull(validator, nameof(validator));

		When(o => validateKey, () =>
		{
			RuleFor(o => o.Key).NotEmpty();
		});

		RuleFor(o => o.Value).SetValidator(validator);
	}
}

/// <summary>
/// Provides dictionary extensions for fluent validation.
/// </summary>
public static class DictionaryFluentValidationExtensions
{
	/// <summary>
	/// Sets a validator for a set of key value pairs.
	/// </summary>
	/// <typeparam name="T">The model type.</typeparam>
	/// <typeparam name="TKey">The key type.</typeparam>
	/// <typeparam name="TValue">The value type.</typeparam>
	/// <typeparam name="TValidator">The validator type.</typeparam>
	/// <param name="builder">The rule builder.</param>
	/// <param name="validator">The validator.</param>
	/// <param name="validateKey">Should the key be validated?</param>
	/// <returns>The rule builder.</returns>
	public static IRuleBuilder<T, KeyValuePair<TKey, TValue>> SetDictionaryValidator<T, TKey, TValue, TValidator>(
		this IRuleBuilder<T, KeyValuePair<TKey, TValue>> builder, 
		TValidator validator,
		bool validateKey = true)
		where TValidator : IValidator<TValue>
	{
		var dictValidator 
			= new DictionaryValidator<TKey, TValue, TValidator>(validator, validateKey);

		return builder.SetValidator(dictValidator);
	}
}