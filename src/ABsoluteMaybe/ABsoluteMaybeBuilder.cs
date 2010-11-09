using System;
using System.Collections.Generic;
using System.Linq;
using ABsoluteMaybe.Identification;
using ABsoluteMaybe.OptionChoosing;
using ABsoluteMaybe.Persistence;
using ABsoluteMaybe.Serialization;
using ABsoluteMaybe.ShortCircuiting;
using ABsoluteMaybe.UserFiltering;

namespace ABsoluteMaybe
{
	public class ABsoluteMaybeBuilder
	{
		private readonly IList<Func<IShortCircuiter>> _shortCircuitFactories = new List<Func<IShortCircuiter>>
		                                                                       	{
		                                                                       		() => new EndedExperimentShortCircuiter()
		                                                                       	};

		private readonly IList<Func<IUserFilter>> _userFilterFactories = new List<Func<IUserFilter>>
		                                                                 	{
		                                                                 		() => new SpiderFilter()
		                                                                 	};

		private Func<IExperimentCommands> _experimentCommandsFactory;
		private Func<IOptionChooser> _optionChooserFactory = () => new RandomOptionChooser();
		private Func<IOptionSerializer> _optionSerializerFactory = () => new ToStringOptionSerializer();
		private Func<IUserIdentification> _userIdentificationFactory = () => new CookieUserIdentification();

		private ABsoluteMaybeBuilder()
		{
		}

		public static ABsoluteMaybeBuilder StoreExperimentsUsing(Func<IExperimentCommands> experimentCommandsFactory)
		{
			return new ABsoluteMaybeBuilder()
				.SetExperimentStorage(experimentCommandsFactory);
		}

		private ABsoluteMaybeBuilder SetExperimentStorage(Func<IExperimentCommands> experimentCommandsFactory)
		{
			_experimentCommandsFactory = experimentCommandsFactory;
			return this;
		}

		public ABsoluteMaybeBuilder SetOptionChooser(Func<IOptionChooser> optionChooserFactory)
		{
			_optionChooserFactory = optionChooserFactory;
			return this;
		}

		public ABsoluteMaybeBuilder SetOptionSerializer(Func<IOptionSerializer> optionSerializerFactory)
		{
			_optionSerializerFactory = optionSerializerFactory;
			return this;
		}

		public ABsoluteMaybeBuilder SetUserIdentification(Func<IUserIdentification> userIdentificationFactory)
		{
			_userIdentificationFactory = userIdentificationFactory;
			return this;
		}

		public ABsoluteMaybeBuilder ClearUserFilters()
		{
			_userFilterFactories.Clear();
			return this;
		}

		public ABsoluteMaybeBuilder AddUserFilter(Func<IUserFilter> userFilterFactory)
		{
			_userFilterFactories.Add(userFilterFactory);
			return this;
		}

		public ABsoluteMaybeBuilder ClearShortCircuiters()
		{
			_shortCircuitFactories.Clear();
			return this;
		}

		public ABsoluteMaybeBuilder AddShortCircuiter(Func<IShortCircuiter> shortCircuiterFactory)
		{
			_shortCircuitFactories.Add(shortCircuiterFactory);
			return this;
		}

		public Func<IABsoluteMaybe> Build()
		{
			return
				() =>
				new DefaultABsoluteMaybe(_experimentCommandsFactory(),
				                         _optionChooserFactory(),
				                         _optionSerializerFactory(),
				                         _shortCircuitFactories.Select(factory => factory()),
				                         _userFilterFactories.Select(factory => factory()),
				                         _userIdentificationFactory()
					);
		}
	}
}