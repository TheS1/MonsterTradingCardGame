﻿@echo off

REM --------------------------------------------------
REM Monster Trading Cards Game
REM --------------------------------------------------
title Monster Trading Cards Game
echo CURL Testing for Monster Trading Cards Game
echo.

REM --------------------------------------------------
echo 1) Create Users (Registration)
REM Create User
curl -X POST http://localhost:8080/register --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}"
echo.
curl -X POST http://localhost:8080/register --header "Content-Type: application/json" -d "{\"Username\":\"altenhof\", \"Password\":\"markus\"}"
echo.


echo should fail:
curl -X POST http://localhost:8080/register --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}"
echo.
curl -X POST http://localhost:8080/register --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"different\"}"
echo. 
echo.

REM --------------------------------------------------
echo 2) Login Users
curl -X POST http://localhost:8080/login --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"daniel\"}"
echo.
curl -X POST http://localhost:8080/login --header "Content-Type: application/json" -d "{\"Username\":\"altenhof\", \"Password\":\"markus\"}"
echo.
curl -X POST http://localhost:8080/login --header "Content-Type: application/json" -d "{\"Username\":\"admin\",    \"Password\":\"admin\"}"
echo.

echo should fail:
curl -X POST http://localhost:8080/login --header "Content-Type: application/json" -d "{\"Username\":\"kienboec\", \"Password\":\"different\"}"
echo.
echo.

REM --------------------------------------------------
echo 3) create packages (done by "admin")
curl -X POST http://localhost:8080/addCards --header "Content-Type: application/json" --header "Authorization: Bearer admin-mtcgToken" -d "[{\"Id\":\"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"e85e3976-7c86-4d06-9a80-641c2019a79f\", \"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Id\":\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"Name\":\"Ork\", \"Damage\": 45.0}, {\"Id\":\"dfdd758f-649c-40f9-ba3a-8657f4b3439f\", \"Name\":\"FireSpell\",    \"Damage\": 25.0}]"
echo.																																																																																		 				    
curl -X POST http://localhost:8080/addCards --header "Content-Type: application/json" --header "Authorization: Bearer admin-mtcgToken" -d "[{\"Id\":\"644808c2-f87a-4600-b313-122b02322fd5\", \"Name\":\"WaterGoblin\", \"Damage\":  9.0}, {\"Id\":\"4a2757d6-b1c3-47ac-b9a3-91deab093531\", \"Name\":\"Dragon\", \"Damage\": 55.0}, {\"Id\":\"91a6471b-1426-43f6-ad65-6fc473e16f9f\", \"Name\":\"WaterSpell\", \"Damage\": 21.0}, {\"Id\":\"4ec8b269-0dfa-4f97-809a-2c63fe2a0025\", \"Name\":\"Ork\", \"Damage\": 55.0}, {\"Id\":\"f8043c23-1534-4487-b66b-238e0c3c39b5\", \"Name\":\"WaterSpell\",   \"Damage\": 23.0}]"
echo.																																																																																		 				    
curl -X POST http://localhost:8080/addCards --header "Content-Type: application/json" --header "Authorization: Bearer admin-mtcgToken" -d "[{\"Id\":\"b017ee50-1c14-44e2-bfd6-2c0c5653a37c\", \"Name\":\"WaterGoblin\", \"Damage\": 11.0}, {\"Id\":\"d04b736a-e874-4137-b191-638e0ff3b4e7\", \"Name\":\"Dragon\", \"Damage\": 70.0}, {\"Id\":\"88221cfe-1f84-41b9-8152-8e36c6a354de\", \"Name\":\"WaterSpell\", \"Damage\": 22.0}, {\"Id\":\"1d3f175b-c067-4359-989d-96562bfa382c\", \"Name\":\"Ork\", \"Damage\": 40.0}, {\"Id\":\"171f6076-4eb5-4a7d-b3f2-2d650cc3d237\", \"Name\":\"RegularSpell\", \"Damage\": 28.0}]"
echo.																																																																																		 				    
curl -X POST http://localhost:8080/addCards --header "Content-Type: application/json" --header "Authorization: Bearer admin-mtcgToken" -d "[{\"Id\":\"ed1dc1bc-f0aa-4a0c-8d43-1402189b33c8\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0, \"Set\":\"Special\"}, {\"Id\":\"65ff5f23-1e70-4b79-b3bd-f6eb679dd3b5\", \"Name\":\"Dragon\", \"Damage\": 50.0, \"Set\":\"Special\"}, {\"Id\":\"55ef46c4-016c-4168-bc43-6b9b1e86414f\", \"Name\":\"WaterSpell\", \"Damage\": 20.0, \"Set\":\"Special\"}, {\"Id\":\"f3fad0f2-a1af-45df-b80d-2e48825773d9\", \"Name\":\"Ork\", \"Damage\": 45.0, \"Set\":\"Special\"}, {\"Id\":\"8c20639d-6400-4534-bd0f-ae563f11f57a\", \"Name\":\"WaterSpell\",   \"Damage\": 25.0, \"Set\":\"Special\"}]"
echo.																																																																																		 				    
curl -X POST http://localhost:8080/addCards --header "Content-Type: application/json" --header "Authorization: Bearer admin-mtcgToken" -d "[{\"Id\":\"d7d0cb94-2cbf-4f97-8ccf-9933dc5354b8\", \"Name\":\"WaterGoblin\", \"Damage\":  9.0, \"Set\":\"Special\"}, {\"Id\":\"44c82fbc-ef6d-44ab-8c7a-9fb19a0e7c6e\", \"Name\":\"Dragon\", \"Damage\": 55.0, \"Set\":\"Special\"}, {\"Id\":\"2c98cd06-518b-464c-b911-8d787216cddd\", \"Name\":\"WaterSpell\", \"Damage\": 21.0, \"Set\":\"Special\"}, {\"Id\":\"951e886a-0fbf-425d-8df5-af2ee4830d85\", \"Name\":\"Ork\", \"Damage\": 55.0, \"Set\":\"Special\"}, {\"Id\":\"dcd93250-25a7-4dca-85da-cad2789f7198\", \"Name\":\"FireSpell\",    \"Damage\": 23.0, \"Set\":\"Special\"}]"
echo.																																																																																		 				    
curl -X POST http://localhost:8080/addCards --header "Content-Type: application/json" --header "Authorization: Bearer admin-mtcgToken" -d "[{\"Id\":\"b2237eca-0271-43bd-87f6-b22f70d42ca4\", \"Name\":\"WaterGoblin\", \"Damage\": 11.0, \"Set\":\"Special\"}, {\"Id\":\"9e8238a4-8a7a-487f-9f7d-a8c97899eb48\", \"Name\":\"Dragon\", \"Damage\": 70.0, \"Set\":\"Special\"}, {\"Id\":\"d60e23cf-2238-4d49-844f-c7589ee5342e\", \"Name\":\"WaterSpell\", \"Damage\": 22.0, \"Set\":\"Special\"}, {\"Id\":\"fc305a7a-36f7-4d30-ad27-462ca0445649\", \"Name\":\"Ork\", \"Damage\": 40.0, \"Set\":\"Special\"}, {\"Id\":\"84d276ee-21ec-4171-a509-c1b88162831c\", \"Name\":\"RegularSpell\", \"Damage\": 28.0, \"Set\":\"Special\"}]"
echo.
echo.

REM --------------------------------------------------
echo 4) acquire packages kienboec
curl -X POST http://localhost:8080/buyPack --header "Content-Type: application/json" --header "Authorization: Bearer kienboec-mtcgToken" -d "{\"Set\":\"Basic\"}"
echo.
curl -X POST http://localhost:8080/buyPack --header "Content-Type: application/json" --header "Authorization: Bearer kienboec-mtcgToken" -d "{\"Set\":\"Basic\"}"
echo.
curl -X POST http://localhost:8080/buyPack --header "Content-Type: application/json" --header "Authorization: Bearer kienboec-mtcgToken" -d "{\"Set\":\"Basic\"}"
echo.
echo.
echo should fail:
curl -X POST http://localhost:8080/buyPack --header "Content-Type: application/json" --header "Authorization: Bearer kienboec-mtcgToken" -d "{\"Set\":\"Basic\"}"

REM --------------------------------------------------
echo 5) acquire packages altenhof
curl -X POST http://localhost:8080/buyPack --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d "{\"Set\":\"Special\"}"
echo.
curl -X POST http://localhost:8080/buyPack --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d "{\"Set\":\"Special\"}"
echo.
curl -X POST http://localhost:8080/buyPack --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d "{\"Set\":\"Special\"}"
echo should fail:
curl -X POST http://localhost:8080/buyPack --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d "{\"Set\":\"Special\"}"


REM --------------------------------------------------
echo 6) add new packages
curl -X POST http://localhost:8080/addCards --header "Content-Type: application/json" --header "Authorization: Bearer admin-mtcgToken" -d "[{\"Id\":\"67f9048f-99b8-4ae4-b866-d8008d00c53d\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"aa9999a0-734c-49c6-8f4a-651864b14e62\", \"Name\":\"RegularSpell\", \"Damage\": 50.0}, {\"Id\":\"d6e9c720-9b5a-40c7-a6b2-bc34752e3463\", \"Name\":\"Knight\", \"Damage\": 20.0}, {\"Id\":\"02a9c76e-b17d-427f-9240-2dd49b0d3bfd\", \"Name\":\"RegularSpell\", \"Damage\": 45.0}, {\"Id\":\"2508bf5c-20d7-43b4-8c77-bc677decadef\", \"Name\":\"FireElf\", \"Damage\": 25.0}]"
echo.
curl -X POST http://localhost:8080/addCards --header "Content-Type: application/json" --header "Authorization: Bearer admin-mtcgToken" -d "[{\"Id\":\"70962948-2bf7-44a9-9ded-8c68eeac7793\", \"Name\":\"WaterGoblin\", \"Damage\":  9.0}, {\"Id\":\"74635fae-8ad3-4295-9139-320ab89c2844\", \"Name\":\"FireSpell\", \"Damage\": 55.0}, {\"Id\":\"ce6bcaee-47e1-4011-a49e-5a4d7d4245f3\", \"Name\":\"Knight\", \"Damage\": 21.0}, {\"Id\":\"a6fde738-c65a-4b10-b400-6fef0fdb28ba\", \"Name\":\"FireSpell\", \"Damage\": 55.0}, {\"Id\":\"a1618f1e-4f4c-4e09-9647-87e16f1edd2d\", \"Name\":\"FireElf\", \"Damage\": 23.0}]"
echo.
curl -X POST http://localhost:8080/addCards --header "Content-Type: application/json" --header "Authorization: Bearer admin-mtcgToken" -d "[{\"Id\":\"2272ba48-6662-404d-a9a1-41a9bed316d9\", \"Name\":\"WaterGoblin\", \"Damage\": 11.0}, {\"Id\":\"3871d45b-b630-4a0d-8bc6-a5fc56b6a043\", \"Name\":\"Dragon\", \"Damage\": 70.0}, {\"Id\":\"166c1fd5-4dcb-41a8-91cb-f45dcd57cef3\", \"Name\":\"Knight\", \"Damage\": 22.0}, {\"Id\":\"237dbaef-49e3-4c23-b64b-abf5c087b276\", \"Name\":\"WaterSpell\", \"Damage\": 40.0}, {\"Id\":\"27051a20-8580-43ff-a473-e986b52f297a\", \"Name\":\"FireElf\", \"Damage\": 28.0}]"
echo.
echo.

REM --------------------------------------------------
echo 7) acquire newly created packages altenhof
curl -X POST http://localhost:8080/buyPack --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d "{\"Set\":\"Basic\"}"
echo.
curl -X POST http://localhost:8080/buyPack --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d "{\"Set\":\"Basic\"}"
echo.
echo should fail (no money):
curl -X POST http://localhost:8080/buyPack --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d "{\"Set\":\"Basic\"}"
echo.
echo.

REM --------------------------------------------------
echo 8) show all acquired cards kienboec
curl -X GET http://localhost:8080/cards --header "Authorization: Bearer kienboec-mtcgToken"
echo should fail (no token)
curl -X GET http://localhost:8080/cards 
echo.
echo.

REM --------------------------------------------------
echo 9) show all acquired cards altenhof
curl -X GET http://localhost:8080/cards --header "Authorization: Bearer altenhof-mtcgToken"
echo.
echo.

REM --------------------------------------------------
echo 10) show unconfigured deck
curl -X GET http://localhost:8080/deck --header "Authorization: Bearer kienboec-mtcgToken"
echo.
curl -X GET http://localhost:8080/deck --header "Authorization: Bearer altenhof-mtcgToken"
echo.
echo.

REM --------------------------------------------------
echo 11) configure deck
curl -X PUT http://localhost:8080/deck --header "Content-Type: application/json" --header "Authorization: Bearer kienboec-mtcgToken" -d "[\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"f8043c23-1534-4487-b66b-238e0c3c39b5\", \"dfdd758f-649c-40f9-ba3a-8657f4b3439f\"]"
echo.
curl -X GET http://localhost:8080/deck --header "Authorization: Bearer kienboec-mtcgToken"
echo.
curl -X PUT http://localhost:8080/deck --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d "[\"b2237eca-0271-43bd-87f6-b22f70d42ca4\", \"f3fad0f2-a1af-45df-b80d-2e48825773d9\", \"44c82fbc-ef6d-44ab-8c7a-9fb19a0e7c6e\", \"55ef46c4-016c-4168-bc43-6b9b1e86414f\"]"
echo.
curl -X GET http://localhost:8080/deck --header "Authorization: Bearer altenhof-mtcgToken"
echo.
echo.
echo should fail and show original from before:
curl -X PUT http://localhost:8080/deck --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d "[\"1cb6ab86-bdb2-47e5-b6e4-68c5ab389334\", \"99f8f8dc-e25e-4a95-aa2c-782823f36e2a\", \"f8043c23-1534-4487-b66b-238e0c3c39b5\", \"f8043c23-1534-4487-b66b-238e0c3c39b5\"]"
echo.
curl -X GET http://localhost:8080/deck --header "Authorization: Bearer altenhof-mtcgToken"
echo.
echo should also fail:
curl -X PUT http://localhost:8080/deck --header "Content-Type: application/json" --header "Authorization: Bearer kienboec-mtcgToken" -d "[\"b2237eca-0271-43bd-87f6-b22f70d42ca4\", \"55ef46c4-016c-4168-bc43-6b9b1e86414f\", \"44c82fbc-ef6d-44ab-8c7a-9fb19a0e7c6e\", \"55ef46c4-016c-4168-bc43-6b9b1e86414f\"]"
echo.


REM --------------------------------------------------
echo 12) show configured deck 
curl -X GET http://localhost:8080/deck --header "Authorization: Bearer kienboec-mtcgToken"
echo.
curl -X GET http://localhost:8080/deck --header "Authorization: Bearer altenhof-mtcgToken"
echo.
echo.


REM --------------------------------------------------
echo 14) edit user data
echo.
curl -X GET http://localhost:8080/myProfile --header "Authorization: Bearer kienboec-mtcgToken"
echo.
curl -X GET http://localhost:8080/myProfile --header "Authorization: Bearer altenhof-mtcgToken"
echo.
curl -X PUT http://localhost:8080/myProfile --header "Content-Type: application/json" --header "Authorization: Bearer kienboec-mtcgToken" -d "{\"Name\": \"Kienboeck\",  \"Bio\": \"me playin...\", \"Image\": \":-)\"}"
echo.
curl -X PUT http://localhost:8080/myProfile --header "Content-Type: application/json" --header "Authorization: Bearer altenhof-mtcgToken" -d "{\"Name\": \"Altenhofer\", \"Bio\": \"me codin...\",  \"Image\": \":-D\"}"
echo.
curl -X GET http://localhost:8080/myProfile --header "Authorization: Bearer Kienboeck-mtcgToken"
echo.
curl -X GET http://localhost:8080/myProfile --header "Authorization: Bearer Altenhofer-mtcgToken"
echo.
echo.
echo should fail:

curl -X PUT http://localhost:8080/myProfile --header "Content-Type: application/json" --header "Authorization: Bearer Kienboeck-mtcgToken" -d "{\"Name\": \"Altenhofer\", \"Bio\": \"me codin...\",  \"Image\": \":-D\"}"
echo.
curl -X GET http://localhost:8080/myProfile  --header "Authorization: Bearer Kienboeck-mtcgToken"
echo.
echo.

REM --------------------------------------------------
echo 16) scoreboard
curl -X GET http://localhost:8080/scoreboard --header "Authorization: Bearer Kienboeck-mtcgToken"
echo.
echo.

REM --------------------------------------------------
echo 17) battle
start /b "kienboec battle" curl -X POST http://localhost:8080/battle --header "Authorization: Bearer Kienboeck-mtcgToken"
start /b "altenhof battle" curl -X POST http://localhost:8080/battle --header "Authorization: Bearer Altenhofer-mtcgToken"
ping localhost -n 10 >NUL 2>NUL


REM --------------------------------------------------
echo 19) scoreboard
curl -X GET http://localhost:8080/scoreboard --header "Authorization: Bearer Kienboeck-mtcgToken"
echo.
echo.

REM --------------------------------------------------
echo 20) trade
echo check trading deals
curl -X GET http://localhost:8080/tradings --header "Authorization: Bearer Kienboeck-mtcgToken"
echo.
echo create trading deal
curl -X POST http://localhost:8080/tradings --header "Content-Type: application/json" --header "Authorization: Bearer Altenhofer-mtcgToken" -d "{\"Id\": \"b2237eca-0271-43bd-87f6-b22f70d42ca4\", \"Type\": \"monster\", \"MinimumDamage\": 15}"
echo.
echo check trading deals
curl -X GET http://localhost:8080/tradings --header "Authorization: Bearer Kienboeck-mtcgToken"
echo.
curl -X GET http://localhost:8080/tradings --header "Authorization: Bearer Altenhofer-mtcgToken"
echo.
echo delete trading deals
curl -X DELETE http://localhost:8080/tradings/1 --header "Authorization: Bearer Altenhofer-mtcgToken"
echo.
echo.

REM --------------------------------------------------
echo 21) check trading deals
echo.
curl -X POST http://localhost:8080/tradings --header "Content-Type: application/json" --header "Authorization: Bearer Kienboeck-mtcgToken" -d "{\"Id\": \"845f0dc7-37d0-426e-994e-43fc3ac83c08\", \"Type\": \"Monster\", \"MinimumDamage\": 15}"
echo check trading deals
curl -X GET http://localhost:8080/tradings  --header "Authorization: Bearer Kienboeck-mtcgToken"

echo.
echo try to trade with yourself (should fail)
curl -X POST http://localhost:8080/tradings/2--header "Content-Type: application/json" --header "Authorization: Bearer Kienboeck-mtcgToken" -d "\"4ec8b269-0dfa-4f97-809a-2c63fe2a0025\""
echo.
echo try to trade 
echo.
curl -X POST http://localhost:8080/tradings/1 --header "Content-Type: application/json" --header "Authorization: Bearer Kienboeck-mtcgToken" -d "{\"Id\":\"d04b736a-e874-4137-b191-638e0ff3b4e7\"}"
echo.
curl -X GET http://localhost:8080/tradings --header "Authorization: Bearer Kienboeck-mtcgToken"
echo.
curl -X GET http://localhost:8080/tradings --header "Authorization: Bearer Altenhofer-mtcgToken"
echo.

REM --------------------------------------------------
echo end...

REM this is approx a sleep 
ping localhost -n 100 >NUL 2>NUL
@echo on
