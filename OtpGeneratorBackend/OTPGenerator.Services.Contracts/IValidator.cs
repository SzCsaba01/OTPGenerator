namespace OTPGenerator.Services.Contracts;
public interface IValidator<T>
{
    public void Validate(T entity);
}
