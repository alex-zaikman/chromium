#ifndef t2kutil_ed6366636df63teddjjjdj7377jj4343ffdefsdsweer4343jjsdefefr
#define t2kutil_ed6366636df63teddjjjdj7377jj4343ffdefsdsweer4343jjsdefefr
#pragma once	
#include <string>




#define CONFIG  "config.ini"
#define CACHE_DIR "Cache"

class utils{
public:

	 static utils& the(){
		 static utils single;
		 return single;
	 }

	 std::wstring cachePath();
	 void WriteURLFromConfig(std::string url);
	 std::string ReadURLFromConfig();
	 std::string getExectablePath();

private:
	utils(void){
		path = preparePath();
	};
	utils(const utils& src);
	utils& operator=(const utils& rhs);

	std::string preparePath();

	//members
	 std::string path;


};


#endif //t2kutil_ed6366636df63teddjjjdj7377jj4343ffdefsdsweer4343jjsdefefr