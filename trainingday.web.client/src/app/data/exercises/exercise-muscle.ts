export enum MusclesEnum {
  Neck = '0',
  Trapezium = '1',
  ShouldersFront = '2',
  ShouldersBack = '3',
  ShouldersMiddle = '4',
  WidestBack = '5',
  MiddleBack = '6',
  ErectorSpinae = '7',
  Chest = '8',
  Abdominal = '9',
  Triceps = '10',
  Biceps = '11',
  Forearm = '12',
  Quadriceps = '13',
  Caviar = '14',
  ShinCamboloid = '15',
  ShinAnteriorTibialis = '16',
  Thighs = '17',
  Buttocks = '18',
  Cardio = '19',
  LowerBack = '20',
}

export const MusclesEnumEnglishLabels: Record<MusclesEnum, string> = {
  [MusclesEnum.Neck]: 'Neck',
  [MusclesEnum.Trapezium]: 'Trapezium',
  [MusclesEnum.ShouldersFront]: 'Front Delta',
  [MusclesEnum.ShouldersBack]: 'Back Delta',
  [MusclesEnum.ShouldersMiddle]: 'Middle Delta',
  [MusclesEnum.WidestBack]: 'Widest',
  [MusclesEnum.MiddleBack]: 'Back',
  [MusclesEnum.ErectorSpinae]: 'Spinal',
  [MusclesEnum.Chest]: 'Chest',
  [MusclesEnum.Abdominal]: 'Abdominal',
  [MusclesEnum.Triceps]: 'Triceps',
  [MusclesEnum.Biceps]: 'Biceps',
  [MusclesEnum.Forearm]: 'Forearm',
  [MusclesEnum.Quadriceps]: 'Quadriceps',
  [MusclesEnum.Caviar]: 'Caviar',
  [MusclesEnum.ShinCamboloid]: 'Camboloid',
  [MusclesEnum.ShinAnteriorTibialis]: 'Anterior tibialis',
  [MusclesEnum.Thighs]: 'Thighs',
  [MusclesEnum.Buttocks]: 'Glute',
  [MusclesEnum.Cardio]: 'Cardio',
  [MusclesEnum.LowerBack]: 'Lower Back',
};

export interface MuscleGroup {
  label: string;
  muscles: MusclesEnum[];
}

export const MUSCLE_GROUPS: MuscleGroup[] = [
  {
    label: 'Shoulders & Neck',
    muscles: [MusclesEnum.Neck, MusclesEnum.Trapezium, MusclesEnum.ShouldersFront, MusclesEnum.ShouldersBack, MusclesEnum.ShouldersMiddle],
  },
  {
    label: 'Back',
    muscles: [MusclesEnum.WidestBack, MusclesEnum.MiddleBack, MusclesEnum.ErectorSpinae, MusclesEnum.LowerBack],
  },
  {
    label: 'Chest',
    muscles: [MusclesEnum.Chest],
  },
  {
    label: 'Core',
    muscles: [MusclesEnum.Abdominal],
  },
  {
    label: 'Arms',
    muscles: [MusclesEnum.Triceps, MusclesEnum.Biceps, MusclesEnum.Forearm],
  },
  {
    label: 'Legs',
    muscles: [MusclesEnum.Quadriceps, MusclesEnum.Thighs, MusclesEnum.Buttocks, MusclesEnum.Caviar, MusclesEnum.ShinCamboloid, MusclesEnum.ShinAnteriorTibialis],
  },
  {
    label: 'Cardio',
    muscles: [MusclesEnum.Cardio],
  },
];
