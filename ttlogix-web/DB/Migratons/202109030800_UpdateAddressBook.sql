
   UPDATE TT_Owner SET Name = 'TOYOTA TSUSHO EUROPE SA Poland' WHERE TT_Owner.Code = 'TESAP'
   UPDATE TT_Owner SET Name = 'TOYOTA TSUSHO EUROPE SA Italy' WHERE TT_Owner.Code = 'TESAI'
   UPDATE TT_Owner SET Name = 'TOYOTA TSUSHO EUROPE SA Hungary' WHERE TT_Owner.Code = 'TESAH'
   UPDATE TT_Owner SET Name = 'TOYOTA TSUSHO EUROPE SA Germany' WHERE TT_Owner.Code = 'TESAG'

   UPDATE TT_AddressBook SET Address1 = 'Via R. Mattioli n.3', Address2 = 'Dosson di Casier, Treviso'
   FROM TT_AddressBook JOIN TT_Owner ON TT_Owner.CompanyCode=TT_AddressBook.CompanyCode AND TT_Owner.PrimaryAddress=TT_AddressBook.Code
   WHERE TT_Owner.Code = 'TESAI'
