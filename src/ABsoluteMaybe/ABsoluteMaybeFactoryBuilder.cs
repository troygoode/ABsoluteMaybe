using System;
using System.Collections.Generic;
using System.Linq;
using ABsoluteMaybe.Identification;
using ABsoluteMaybe.OptionChoosing;
using ABsoluteMaybe.Persistence;
using ABsoluteMaybe.Serialization;
using ABsoluteMaybe.UserFiltering;

namespace ABsoluteMaybe
{
	public class ABsoluteMaybeFactoryBuilder
	{
		private readonly IList<Func<IUserFilter>> _userFilterFactories = new List<Func<IUserFilter>>
		                                                                 	{
		                                                                 		() => new SpiderFilter()
		                                                                 	};

		private Func<IExpirementRepository> _expirementRepositoryFactory;
		private Func<IOptionChooser> _optionChooserFactory = () => new RandomOptionChooser();
		private Func<IOptionSerializer> _optionSerializerFactory = () => new ToStringOptionSerializer();
		private Func<IUserIdentification> _userIdentificationFactory = () => new IpAddressUserIdentification();

		public ABsoluteMaybeFactoryBuilder(string pathToXmlStorage)
			: this(() => new XmlExpirementRepository(pathToXmlStorage))
		{
		}

		public ABsoluteMaybeFactoryBuilder(Func<IExpirementRepository> expirementRepositoryFactory)
		{
			SetExpirementRepository(expirementRepositoryFactory);
		}

		public ABsoluteMaybeFactoryBuilder SetExpirementRepository(Func<IExpirementRepository> expirementRepositoryFactory)
		{
			_expirementRepositoryFactory = expirementRepositoryFactory;
			return this;
		}

		public ABsoluteMaybeFactoryBuilder SetOptionChooser(Func<IOptionChooser> optionChooserFactory)
		{
			_optionChooserFactory = optionChooserFactory;
			return this;
		}

		public ABsoluteMaybeFactoryBuilder SetOptionSerializer(Func<IOptionSerializer> optionSerializerFactory)
		{
			_optionSerializerFactory = optionSerializerFactory;
			return this;
		}

		public ABsoluteMaybeFactoryBuilder SetUserIdentification(Func<IUserIdentification> userIdentificationFactory)
		{
			_userIdentificationFactory = userIdentificationFactory;
			return this;
		}

		public ABsoluteMaybeFactoryBuilder ClearUserFilters()
		{
			_userFilterFactories.Clear();
			return this;
		}

		public ABsoluteMaybeFactoryBuilder AddUserFilter(Func<IUserFilter> userFilterFactory)
		{
			_userFilterFactories.Add(userFilterFactory);
			return this;
		}

		public Func<IABsoluteMaybe> Build()
		{
			return
				() =>
				new DefaultABsoluteMaybe(_expirementRepositoryFactory(),
				                         _optionChooserFactory(),
				                         _optionSerializerFactory(),
				                         _userIdentificationFactory(),
				                         _userFilterFactories.Select(factory => factory()));
		}
	}
}