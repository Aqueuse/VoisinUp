DROP TABLE IF EXISTS "User", "Voisinage", "VoisinageSuccess", "Asset", "UserAssets", "GridAssets", "UserQuests", "Quest", "QuestCategories", "QuestCategory", "QuestCategoryDetails", "Success", "QuestComment", "GridAssets" cascade;

CREATE TABLE "User" (
  "UserId" VARCHAR(36) PRIMARY KEY,
  "Name" VARCHAR(100) NOT NULL,
  "VoisinageId" INT,
  "Commune" VARCHAR(100) NOT NULL,
  "Country" VARCHAR(100) NOT NULL,
  "AvatarUrl" VARCHAR(255),
  "Bio" TEXT,
  "BricksQuantity" INT DEFAULT 0,
  "CakesQuantity" INT DEFAULT 0,
  "Email" VARCHAR(255) UNIQUE NOT NULL,
  "PasswordHash" TEXT NOT NULL,
  "BirthDate" DATE,
  "CreationDate" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  "LastLogin" TIMESTAMP
);

CREATE TABLE "Voisinage" (
  "VoisinageId" SERIAL PRIMARY KEY,
  "Name" VARCHAR(100) NOT NULL,
  "Commune" VARCHAR(100) NOT NULL,
  "Country" VARCHAR(50) NOT NULL DEFAULT 'France'
);

CREATE TABLE "VoisinageSuccess" (
  "VoisinageId" INT,
  "SuccessId" INT,
  "DateEarned" TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY("VoisinageId","SuccessId")
);

CREATE TABLE "Asset" (
  "AssetId" VARCHAR(100) PRIMARY KEY,
  "AssetName" VARCHAR(100) NOT NULL,
  "AssetDescription" TEXT,
  "AssetCategory" VARCHAR(36) NOT NULL,  
  "Cost" INT DEFAULT 0
);

CREATE TABLE "UserAssets" (
  "UserId" VARCHAR(36),
  "AssetId" VARCHAR(36),
  "Coordinates" VARCHAR(255) NOT NULL,
  PRIMARY KEY("UserId","AssetId")
);

CREATE TABLE "Quest" (
 "QuestId" VARCHAR(36) PRIMARY KEY,
 "CreatedBy" VARCHAR(36),
 "VoisinageId" INT,
 "Name" VARCHAR(100) NOT NULL,
 "Description" TEXT,
 "DateCreated" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE "QuestCategory" (
    "CategoryId" SERIAL PRIMARY KEY,
    "Name" VARCHAR(100) NOT NULL,
    "Description" TEXT,
    "ImageUrl" VARCHAR(255)
);

CREATE TABLE "QuestCategories" (
   "QuestId" VARCHAR(36), -- Ref to Quest.QuestId
   "CategoryId" INT, -- Ref to QuestCategory.CategoryId
   PRIMARY KEY("CategoryId","QuestId")
);

CREATE TABLE "UserQuests" (
  "QuestId" VARCHAR(36), -- Ref to Quest.QuestId
  "UserId" VARCHAR(36), -- Ref to User.UserId
  PRIMARY KEY("UserId","QuestId")
);

CREATE TABLE "Success" (
  "SuccessId" SERIAL PRIMARY KEY,
  "Name" VARCHAR(100) NOT NULL,
  "Description" TEXT,
  "ImageUrl" VARCHAR(255)
);

CREATE TABLE "QuestComment" (
  "CommentId" VARCHAR(36) PRIMARY KEY,
  "QuestId" VARCHAR(36) NOT NULL,
  "UserId" VARCHAR(36) NOT NULL,
  "Content" TEXT NOT NULL,
  "CreatedAt" TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

ALTER TABLE "User" ADD FOREIGN KEY ("VoisinageId") REFERENCES "Voisinage" ("VoisinageId");
ALTER TABLE "VoisinageSuccess" ADD FOREIGN KEY ("VoisinageId") REFERENCES "Voisinage" ("VoisinageId");
ALTER TABLE "VoisinageSuccess" ADD FOREIGN KEY ("SuccessId") REFERENCES "Success" ("SuccessId");
ALTER TABLE "UserAssets" ADD FOREIGN KEY ("UserId") REFERENCES "User" ("UserId");
ALTER TABLE "UserAssets" ADD FOREIGN KEY ("AssetId") REFERENCES "Asset" ("AssetId");
ALTER TABLE "Quest" ADD FOREIGN KEY ("CreatedBy") REFERENCES "User" ("UserId") ON DELETE SET NULL ("CreatedBy");

ALTER TABLE "UserQuests" ADD FOREIGN KEY ("QuestId") REFERENCES "Quest" ("QuestId") ON DELETE CASCADE;
ALTER TABLE "UserQuests" ADD FOREIGN KEY ("UserId") REFERENCES "User" ("UserId");

ALTER TABLE "QuestCategories" ADD FOREIGN KEY ("QuestId") REFERENCES "Quest" ("QuestId") ON DELETE CASCADE;
ALTER TABLE "QuestCategories" ADD FOREIGN KEY ("CategoryId") REFERENCES "QuestCategory" ("CategoryId");

ALTER TABLE "QuestComment" ADD FOREIGN KEY ("QuestId") REFERENCES "Quest" ("QuestId") ON DELETE CASCADE;
ALTER TABLE "QuestComment" ADD FOREIGN KEY ("UserId") REFERENCES "User" ("UserId") ON DELETE CASCADE;

-- Index pour booster les recherches
CREATE INDEX "idx_QuestComment_QuestId" ON "QuestComment" ("QuestId");
CREATE INDEX "idx_QuestComment_UserId" ON "QuestComment" ("UserId");

INSERT INTO "Voisinage" ("Name", "Commune", "Country")
VALUES ('Ney', 'Angers', 'France');

INSERT INTO "QuestCategory" ("CategoryId", "Name", "Description") VALUES
 (0, 'Solidarité', 'Aide aux voisins : courses, réparations, accompagnement des personnes isolées.'),
 (1, 'Nettoyage', 'Nettoyage des rues, ramassage des déchets, recyclage et tri.'),
 (2, 'Nature', 'Plantation d’arbres, protection de la biodiversité, entretien des espaces verts.'),
 (3, 'Culture', 'Ateliers artistiques, clubs de lecture, événements culturels locaux.'),
 (4, 'Sport', 'Activités sportives, tournois, défis physiques.'),
 (5, 'Développement', 'Amélioration du quartier : réparations, embellissement, installation d’équipements.'),
 (6, 'Découverte', 'Exploration et mise en valeur du patrimoine local, randonnées, défis découverte.'),
 (7, 'Fête', 'Organisation d’événements festifs, repas de quartier, soirées à thème.'),
 (8, 'Accueil', 'Accueil des nouveaux voisins, intégration sociale.')
ON CONFLICT ("CategoryId") DO UPDATE
    SET "Name" = EXCLUDED."Name",
        "Description" = EXCLUDED."Description";
