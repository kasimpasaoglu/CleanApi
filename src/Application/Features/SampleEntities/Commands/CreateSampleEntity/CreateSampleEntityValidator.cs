namespace Application.Features.SampleEntities.Commands.CreateSampleEntity;

public class CreateSampleEntityValidator : AbstractValidator<CreateSampleEntityCommand>
{
    public CreateSampleEntityValidator()
    {
        RuleFor(x => x.Description)
            .Length(ValidationRules.SampleMinDescriptionLength, ValidationRules.SampleMaxDescriptionLength)
            .When(x => x.Description is not null);
    }
}