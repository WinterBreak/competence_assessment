namespace CompetenceAssessment.Core.Specifications;

public class SpecificationBuilderBase<T> where T : class
{
    private SpecificationBase<T> _currentSpecification = new EmptySpecification<T>();
    private bool _isOrSpecification;
    private bool _isNotSpecification;

    protected SpecificationBuilderBase() { }
    
    public SpecificationBase<T> Build() => _currentSpecification;
    
    public SpecificationBuilderBase<T> And()
    {
        _isOrSpecification = false;
        return this;
    }

    public SpecificationBuilderBase<T> Or()
    {
        _isOrSpecification = true;
        return this;
    }

    public SpecificationBuilderBase<T> Not()
    {
        _isNotSpecification = true;
        return this;
    }
    
    public SpecificationBuilderBase<T> AppendSpecification(SpecificationBase<T> specification)
    {
        if (_currentSpecification is null)
        {
            _currentSpecification = specification;
        }
        else
        {
            _currentSpecification = _isOrSpecification
                ? new OrSpecification<T>(_currentSpecification, specification)
                : new AndSpecification<T>(_currentSpecification, specification);
        }

        if (_isNotSpecification)
        {
            _currentSpecification = new NotSpecification<T>(_currentSpecification);
        }

        _isNotSpecification = false;
        _isOrSpecification = false;
        return this;
    }
}