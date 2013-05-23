#include  <t2ku.h>
#include "cefclient/cefclient.h"
#include <vector>
#include <sstream>
#include <stdio.h>
#include <iosfwd>
#include <cstdlib>
#include <iostream>
#include <fstream>
#include <libloaderapi.h>
#include <sys/stat.h>
#include <direct.h>


void utils::WriteURLFromConfig( std::string url )
{
	std::string ini = path+CONFIG;

	std::string line;
	std::ofstream myfile;
	myfile.open(ini);
	myfile << url << std::endl;
	myfile.close();
}

std::string utils::ReadURLFromConfig()
{
	std::string ini =path+CONFIG;

	std::string line;
	std::ifstream myfile (ini);
	if (myfile.is_open())
	{
		bool onse=true;
		while (onse && myfile.good() )
		{
			getline (myfile,line);
			onse=false;
			return line;
		}
		myfile.close();
	}
	//default
	return "http://www.timetoknow.com/";
}

std::string utils::getExectablePath()
{
	std::vector<char> executablePath(MAX_PATH);

	// Try to get the executable path with a buffer of MAX_PATH characters.
	DWORD result = ::GetModuleFileNameA(
		nullptr, &executablePath[0], static_cast<DWORD>(executablePath.size())
		);

	// As long the function returns the buffer size, it is indicating that the buffer
	// was too small. Keep enlarging the buffer by a factor of 2 until it fits.
	while(result == executablePath.size()) {
		executablePath.resize(executablePath.size() * 2);
		result = ::GetModuleFileNameA(
			nullptr, &executablePath[0], static_cast<DWORD>(executablePath.size())
			);
	}

	// If the function returned 0, something went wrong
	if(result == 0) {
		throw std::runtime_error("GetModuleFileName() failed");
	}

	// We've got the path, construct a standard string from it
	return std::string(executablePath.begin(), executablePath.begin() + result);
}


std::string utils::preparePath()
{
	std::string p = getExectablePath();
	unsigned found = p.find_last_of("/\\");
	p=p.substr(0,found);
	p+="\\";
	return p;
}

std::wstring utils::cachePath()
{
	std::string c = path+CACHE_DIR;
	int rc =_mkdir(c.c_str()); //not a must will create anyway
	return std::wstring (c.begin(),c.end());
}


