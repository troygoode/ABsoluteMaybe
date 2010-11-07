require 'fileutils'

def copy_files(from, to, filename, extensions)
	extensions.each do |ext|
		FileUtils.cp "#{from}#{filename}.#{ext}", "#{to}#{filename}.#{ext}"
	end
end

task :prepare_package_core => :release do
  output_directory_core = './packaging/ABsoluteMaybe/lib/40/'
  FileUtils.mkdir_p output_directory_core

  copy_files './src/ABsoluteMaybe/bin/Release/', output_directory_core, 'ABsoluteMaybe', ['dll', 'pdb', 'xml']
end

exec :package_core => :prepare_package_core do |cmd|
  cmd.path_to_command = 'packaging/NuPack-CTP2.exe'
  cmd.parameters [
  	'pack',
    'packaging\\ABsoluteMaybe\\ABsoluteMaybe.nuspec',
    '-o packaging\\ABsoluteMaybe'
  ]
end

task :prepare_package_mvc => :release do
  output_directory_mvc_content_area = './packaging/ABsoluteMaybe.Mvc/content/Areas/ABsoluteMaybeDashboard/'
  output_directory_mvc_content_controllers = './packaging/ABsoluteMaybe.Mvc/content/Areas/ABsoluteMaybeDashboard/Controllers/'
  output_directory_mvc_content_models = './packaging/ABsoluteMaybe.Mvc/content/Areas/ABsoluteMaybeDashboard/Models/'
  output_directory_mvc_content_views = './packaging/ABsoluteMaybe.Mvc/content/Areas/ABsoluteMaybeDashboard/Views/Dashboard/'
  FileUtils.mkdir_p output_directory_mvc_content_area
  FileUtils.mkdir_p output_directory_mvc_content_controllers
  FileUtils.mkdir_p output_directory_mvc_content_models
  FileUtils.mkdir_p output_directory_mvc_content_views

  FileUtils.cp './src/ABsoluteMaybe.SampleWebsite.MVC2/Areas/ABsoluteMaybeDashboard/Controllers/DashboardController.cs',
               output_directory_mvc_content_controllers + 'DashboardController.cs.pp'
  FileUtils.cp './src/ABsoluteMaybe.SampleWebsite.MVC2/Areas/ABsoluteMaybeDashboard/Models/DashboardIndexViewModel.cs',
               output_directory_mvc_content_models + 'DashboardIndexViewModel.cs.pp'
  FileUtils.cp './src/ABsoluteMaybe.SampleWebsite.MVC2/Areas/ABsoluteMaybeDashboard/Views/Web.config',
               output_directory_mvc_content_views + '../Web.config'
  FileUtils.cp './src/ABsoluteMaybe.SampleWebsite.MVC2/Areas/ABsoluteMaybeDashboard/Views/Dashboard/Index.aspx',
               output_directory_mvc_content_views + 'Index.aspx.pp'
  FileUtils.cp './src/ABsoluteMaybe.SampleWebsite.MVC2/Areas/ABsoluteMaybeDashboard/ABsoluteMaybeConfiguration.cs',
               output_directory_mvc_content_area + 'ABsoluteMaybeConfiguration.cs.pp'
  FileUtils.cp './src/ABsoluteMaybe.SampleWebsite.MVC2/Areas/ABsoluteMaybeDashboard/ABsoluteMaybeDashboardAreaRegistration.cs',
               output_directory_mvc_content_area + 'ABsoluteMaybeDashboardAreaRegistration.cs.pp'
  
  replace_namespace output_directory_mvc_content_area + 'ABsoluteMaybeConfiguration.cs.pp'
  replace_namespace output_directory_mvc_content_area + 'ABsoluteMaybeDashboardAreaRegistration.cs.pp'
  replace_namespace output_directory_mvc_content_controllers + 'DashboardController.cs.pp'
  replace_namespace output_directory_mvc_content_models + 'DashboardIndexViewModel.cs.pp'
  replace_namespace output_directory_mvc_content_views + 'Index.aspx.pp'
end

def replace_namespace(file_path)
  text = File.read file_path
  File.open file_path, 'w' do |file|
  	file.puts text.gsub /ABsoluteMaybe\.SampleWebsite\.MVC2/, '$rootnamespace$'
  end
end

exec :package_mvc => :prepare_package_mvc do |cmd|
  cmd.path_to_command = 'packaging/NuPack-CTP2.exe'
  cmd.parameters [
  	'pack',
    'packaging\\ABsoluteMaybe.Mvc\\ABsoluteMaybe.Mvc.nuspec',
    '-o packaging\\ABsoluteMaybe.Mvc'
  ]
end

task :package => [:package_core, :package_mvc] do
end

task :clean_packages do
	FileUtils.rm_r './packaging/ABsoluteMaybe/lib/' unless not File.directory? './packaging/ABsoluteMaybe/lib/'
	FileUtils.rm Dir.glob './packaging/ABsoluteMaybe/*.nupkg'

	FileUtils.rm_r './packaging/ABsoluteMaybe.Mvc/lib/' unless not File.directory? './packaging/ABsoluteMaybe.Mvc/lib/'
	FileUtils.rm_r './packaging/ABsoluteMaybe.Mvc/content/' unless not File.directory? './packaging/ABsoluteMaybe.Mvc/content/'
	FileUtils.rm Dir.glob './packaging/ABsoluteMaybe.Mvc/*.nupkg'
end