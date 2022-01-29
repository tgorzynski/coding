#include<iostream>
#include<cstring>
using namespace std;

void coding(char *word)
{
	int lgth = strlen(word); //dlugosc wprowadzonego wyrazu
	
	for(int i=0; i<dl-1; i+=2) //petla for
	{
		char cd = word[i];
		napis[i] = word[i+1]; // przesuniecie znakow wyrazu
		napis[i+1] = cd;
	}
}

int main()
{
	char word[1000];
	
	cout<<"Provide your word: ";
	cin.getline(word, 100);
	
	coding(word); // wykorzystanie metody szyfrowania
	
	cout<<"Word has been coded: ";
	cout<<word<<endl;
	
	
	coding(word); // ponowne przestawienie dajace deszyfracje wyrazu
	
	cout<<"Decoding completed: ";
	cout<<word<<endl;

	return 0;
}
