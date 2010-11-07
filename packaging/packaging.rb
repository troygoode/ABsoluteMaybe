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
  output_directory_mvc_lib = './packaging/ABsoluteMaybe.Mvc/lib/40/'
  output_directory_mvc_content_controllers = './packaging/EngageNet.Mvc/content/Controllers/'
  output_directory_mvc_content_views = './packaging/EngageNet.Mvc/content/Views/Engage/'
  FileUtils.mkdir_p output_directory_mvc_lib
  FileUtils.mkdir_p output_directory_mvc_content_controllers
  FileUtils.mkdir_p output_directory_mvc_content_views

  copy_files './src/EngageNet.Mvc/bin/Release/', output_directory_mvc_lib, 'EngageNet.Mvc', ['dll', 'pdb', 'xml']
  FileUtils.cp './src/EngageNet.SampleWebsiteMvc2/Controllers/EngageController.cs',
               output_directory_mvc_content_controllers + 'EngageController.cs.pp'
  copy_files './src/EngageNet.SampleWebsiteMvc2/Views/Engage/', output_directory_mvc_content_views, 'LogOn', ['aspx']
  copy_files './src/EngageNet.SampleWebsiteMvc2/Views/Engage/', output_directory_mvc_content_views, 'LogOnCancelled', ['aspx']
  copy_files './src/EngageNet.SampleWebsiteMvc2/Views/Engage/', output_directory_mvc_content_views, 'LogOnSuccess', ['aspx']
  
  text = File.read output_directory_mvc_content_controllers + 'EngageController.cs.pp'
  File.open output_directory_mvc_content_controllers + 'EngageController.cs.pp', 'w' do |file|
  	file.puts text.gsub /EngageNet\.SampleWebsiteMvc2/, '$rootnamespace$'
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