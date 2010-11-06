using System;
using ABsoluteMaybe.Identification;
using ABsoluteMaybe.OptionChoosing;
using ABsoluteMaybe.Persistence;
using ABsoluteMaybe.Serialization;

namespace ABsoluteMaybe
{
	public class AbsoluteMaybeFactoryBuilder
	{
		private Func<IExpirementRepository> _expirementRepositoryFactory;
		private Func<IOptionChooser> _optionChooserFactory = () => new RandomOptionChooser();
		private Func<IOptionSerializer> _optionSerializerFactory = () => new ToStringOptionSerializer();
		private Func<IUserIdentification> _userIdentificationFactory = () => new IpAddressUserIdentification();

		public AbsoluteMaybeFactoryBuilder(string pathToXmlStorage)
			: this(() => new XmlExpirementRepository(pathToXmlStorage))
		{
		}

		public AbsoluteMaybeFactoryBuilder(Func<IExpirementRepository> expirementRepositoryFactory)
		{
			SetExpirementRepository(expirementRepositoryFactory);
		}

		public AbsoluteMaybeFactoryBuilder SetExpirementRepository(Func<IExpirementRepository> expirementRepositoryFactory)
		{
			_expirementRepositoryFactory = expirementRepositoryFactory;
			return this;
		}

		public AbsoluteMaybeFactoryBuilder SetOptionChooser(Func<IOptionChooser> optionChooserFactory)
		{
			_optionChooserFactory = optionChooserFactory;
			return this;
		}

		public AbsoluteMaybeFactoryBuilder SetOptionSerializer(Func<IOptionSerializer> optionSerializerFactory)
		{
			_optionSerializerFactory = optionSerializerFactory;
			return this;
		}

		public AbsoluteMaybeFactoryBuilder SetUserIdentification(Func<IUserIdentification> userIdentificationFactory)
		{
			_userIdentificationFactory = userIdentificationFactory;
			return this;
		}

		public Func<IABsoluteMaybe> Build()
		{
			return
				() =>
				new DefaultABsoluteMaybe(_expirementRepositoryFactory(),
				                         _optionChooserFactory(),
				                         _optionSerializerFactory(),
				                         _userIdentificationFactory());
		}
	}
}