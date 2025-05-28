@echo off
call pac auth clear
call pac auth create --name boruto --environment https://boruto-kipon.crm4.dynamics.com --applicationId 5b87d456-3618-4d4d-927f-c8bdd2440f53 --clientSecret "TwC8Q~DSPDqQQtdPAQ2F3IOGNTvReoZLSAv7odiG" --tenant ec4b2478-41f3-4a0b-b5cb-fd8017819f23
call pac auth list
	