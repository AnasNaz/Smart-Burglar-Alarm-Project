//
//  main.c
//  Smart_Burglar_Alarm
//
//  Created by NIHAR MODI on 22/01/20.
//  Copyright © 2020 NIHAR MODI. All rights reserved.
//

#include <stdio.h>
#include <string.h>
#define encpassword "12345"
#define sizearr 30
void checkpassword()     // Function to ask user to enter the password and encrypt it to match the encrypted password
{
    char pass[sizearr];
    int flag=0,count=3;
    do
    {
        printf("Enter the password to enter the building :::: ");
        scanf("%s",pass);
        if (strcmp(pass,encpassword)==0)
        {
            printf("Password Matched, Welcome \n");
            flag=1;
        }
        else
        {
            printf("Wrong Password Entered : ----->  ");
            count--;
            printf("You have %d attempts left \n",count);
            flag = 0;
        }
    } while(flag==0 && count!=0);
    if(count == 0)
    {
        printf("\n Wrong Attempts \n Alarm Activated .... \n\n");
        exit(0);
    }
}
int main()
{
    checkpassword();
    return 0;
}
    
