USE ProjectKB;

ALTER TABLE scores
	ADD selectionMode TINYINT UNSIGNED NOT NULL DEFAULT 1;

CREATE INDEX scores_preset_level_index
	ON scores (preset, level);