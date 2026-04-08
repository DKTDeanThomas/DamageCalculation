using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;


public class DamageCalc : MonoBehaviour
{
    private float attackerStrength;
    private float attackermagic;
    private float TargetDefense;
    private float attackerLuck;
    private float attackerAgility;
    private float targetLuck;
    private float targetAgility;

    private float targetDefenseStage;
    private float attackerAttackStage;
    private float targetSpeedStage;
    private float attackerSpeedStage;

    private int attackerLevel;
    private int defenderLevel;
    private float guardMod;
    private float MagicCharge;
    private float StrengthCharge;


    private float attackerStat;
    private float spellOffense;
    private float spellBase;
    private float spellMod;
    private float spellLC;
    private float spellSumFactor;
    private float spellMin;
    private float spellMax;
    private float spellPower;
    private float spellEfficacy;

    private float critRoot;
    private float critBase;
    private float critLC;
    private float critRate;
    private float basicCritRate;
    private float spellCritRate;

    private float accuracyRoot;
    private float accuracyBase;
    private float accuracyLC;
    private float accuracyRate;
    private float basicAccuracyRate;
    private float spellAccuracyRate;
    private float accuracyMOD;

    public float difficulty;

    // Losing Resources

    public void LoseSP(GameObject user, int cost)
    {
        if (user.GetComponent<PartyStats>() != null)
        {
            user.GetComponent<PartyStats>().LoseSP(cost);
        }
        else if (user.GetComponent<EnemyStats>() != null)
        {
            user.GetComponent<EnemyStats>().LoseSP(cost);
        }
    }

    public void LoseHP(GameObject user, int hpCost)
    {
        if (user.GetComponent<PartyStats>() != null)
        {
            user.GetComponent<PartyStats>().TakeDamage(hpCost);
        }
        else if (user.GetComponent<EnemyStats>() != null)
        {
            //user.GetComponent<EnemyStats>().TakeDamage(hpCost);
        }
    }

    public void LoseIP(GameObject user, int cost)
    {
        if (user.GetComponent<PartyStats>() != null)
        {
            user.GetComponent<PartyStats>().ConsumeIP(cost);
        }
        else if (user.GetComponent<EnemyStats>() != null)
        {
            //user.GetComponent<EnemyStats>().TakeDamage(hpCost);
        }
    }

    // Magic Charge

    private IEnumerator SpawnDBMFX(GameObject target, GameObject FX)
    {
        yield return new WaitForSecondsRealtime(1);
        GameObject skillFX = Instantiate(FX, target.transform.position, target.transform.rotation);

        yield return new WaitForSecondsRealtime(1);

        Destroy(skillFX);
    }

    public void ApplyDoubleMag(GameObject user, GameObject target, GameObject FX)
    {
        StartCoroutine(SpawnDBMFX(target, FX));

        if (user.GetComponent<PartyStats>() != null)
        {
            user.GetComponent<PartyStats>().Focus();
        }

        else if (user.GetComponent<EnemyStats>() != null)
        {
            
        }   
    }

    // Healing Player

    private IEnumerator SpawnHealFX(GameObject target, GameObject FX)
    {
        yield return new WaitForSecondsRealtime(1);
        GameObject skillFX = Instantiate(FX, target.transform.position, target.transform.rotation);

        yield return new WaitForSecondsRealtime(1);

        Destroy(skillFX);
    }

    public void ApplyHeal(GameObject healer, GameObject target, int power, int cost, GameObject FX)
    {
        float toHeal = target.GetComponent<PartyStats>().memMaxHP * (power / 100f);
        int roundedHeal = Mathf.RoundToInt(toHeal);
        target.GetComponent<PartyStats>().Restore(roundedHeal);

        StartCoroutine(SpawnHealFX(target, FX));
    }

    // Debuffing

    public IEnumerator SpawnDisenchantVFX(GameObject Target, GameObject FX)
    {
        yield return new WaitForSecondsRealtime(1);

        GameObject skillFX = Instantiate(FX, Target.transform.position, Target.transform.rotation);
        CameraShakeManager.Instance.Shake(0.5f);


        yield return new WaitForSecondsRealtime(1);
        GameObject.Find("GameManager").GetComponent<CombatManager>().UpdateEnchantStatus();

        Destroy(skillFX);
    }

    public void ApplyDisenchant(GameObject user, GameObject Target, Skills.BuffType type, int cost, GameObject FX)
    {
        if (user.GetComponent<PartyStats>() != null)
        {
            Target.GetComponent<EnemyStats>().Disenchant(type);
        }
        else if (user.GetComponent<EnemyStats>() != null)
        {
            Target.GetComponent<PartyStats>().Disenchant(type);
        }

        StartCoroutine(SpawnDisenchantVFX(Target, FX));

    }

    // Damage Calculation

    public IEnumerator SpawnSpellVFX(GameObject Attacker, GameObject Target, GameObject FX, Skills.MagElement element)
    {
        // Spell VFX Spawned
        yield return new WaitForSecondsRealtime(1);

        GameObject skillFX = Instantiate(FX, Target.transform.position, Target.transform.rotation);
        CameraShakeManager.Instance.Shake(1f);

        PartyStats attackerParty = Attacker.GetComponent<PartyStats>();
        EnemyStats attackerEnemy = Attacker.GetComponent<EnemyStats>();

        PartyStats targetParty = Target.GetComponent<PartyStats>();
        EnemyStats targetEnemy = Target.GetComponent<EnemyStats>();


        if (attackerParty != null)
        {
            targetEnemy.DamageAnim();
            targetEnemy.TakeDamage();
            targetEnemy.enableSelect();
        }
        else if (attackerEnemy != null)
        {
            targetParty.DamageAnim();
            targetParty.TakeDamageNEW();
            //targetParty.enableSelect();
            targetParty.HPBarDisplay();
        }

        CombatManager cM = GameObject.Find("GameManager").GetComponent<CombatManager>();

        if (attackerParty != null)
        {
            if (!targetEnemy.hasMissed)
            {
                if (targetEnemy.weakness.Contains(element))
                {
                    cM.currentEnemy.GetComponent<EnemyStats>().WeakAnim();
                }
                else if (targetEnemy.resist.Contains(element))
                {
                    cM.currentEnemy.GetComponent<EnemyStats>().ResAnim();
                }
            }
        }
        else if (attackerEnemy != null)
        {
            if (!targetParty.hasMissed)
            {
                if (targetParty.weakness.Contains(element))
                {
                    cM.currentEnemy.GetComponent<PartyStats>().WeakAnim();
                }
                else if (targetParty.resist.Contains(element))
                {
                    cM.currentEnemy.GetComponent<PartyStats>().ResAnim();
                }
            }
        }

        yield return new WaitForSecondsRealtime(1);

        Destroy(skillFX);
    }

    public void SpellAttackCalculation(GameObject Attacker, GameObject Target, Skills.MagElement element, int baseDamage, int accuracy, int crit, int cost, GameObject FX, Skills.Type type)
    {
        var attackerParty = Attacker.GetComponent<PartyStats>();
        var attackerEnemy = Attacker.GetComponent<EnemyStats>();

        var targetParty = Target.GetComponent<PartyStats>();
        var targetEnemy = Target.GetComponent<EnemyStats>();

        MagicCharge = 1;

        // If Player is attacking
        if (attackerParty != null)
        {
            attackerStrength = attackerParty.mythosStrength;
            attackermagic = attackerParty.mythosMagic;
            TargetDefense = targetEnemy.nightmareEndurance;
            attackerLevel = attackerParty.playerLevel;
            defenderLevel = targetEnemy.nightmareLevel;

            if (type == Skills.Type.Magic && attackerParty.isFocused)
            {
                MagicCharge = 2;
                attackerParty.FocusDisable();
            }
        }

        // If Enemy is attacking
        else if (attackerEnemy != null)
        {
            attackerStrength = attackerEnemy.nightmareStrength;
            attackermagic = attackerEnemy.nightmareMagic;
            TargetDefense = targetParty.mythosEndurance;
            attackerLevel = attackerEnemy.nightmareLevel;
            defenderLevel = targetParty.playerLevel;
        }

        spellOffense = 0;
        spellBase = 0;
        attackerStat = 0;

        // Determining the root value
        int root = attackerLevel + 10;

        // Checking if the Spell is Physical or Magic
        if (type == Skills.Type.Physical)
        {
            attackerStat = attackerStrength;
        }
        else if (type == Skills.Type.Magic)
        {
            attackerStat = attackermagic;
        }

        // Determining Offense Value
        if (attackerStat <= root)
        {
            spellOffense = root + attackerStat;
        }
        else
        {
            spellOffense = ((Mathf.Sqrt(attackerStat - root)) / 2) + (attackerStat / 2) + (root * 3) / 2;
        }

        // Determining Base value
        SpellAttackBase(Attacker, Target, element, baseDamage, accuracy, crit, FX);
    }

    public void SpellAttackBase(GameObject Attacker, GameObject Target, Skills.MagElement element, int baseDamage, int accuracy, int crit, GameObject FX)
    {

        // For enemies with high endurance
        if (spellOffense - TargetDefense <= (spellOffense / 2))
        {
            spellBase = ((spellOffense * 2) / 3) - (TargetDefense / 3) - Mathf.Sqrt((TargetDefense - (spellOffense / 2)) / 3);
        }

        // For enemies with low endurance
        else if (spellOffense - TargetDefense > ((spellOffense * 3) / 4))
        {
            spellBase = ((spellOffense * 5) / 6) - (TargetDefense / 3) + Mathf.Sqrt(((spellOffense / 4) - TargetDefense) / 3);
        }

        else
        {
            spellBase = spellOffense - TargetDefense;
        }

        SpellAttackModified(Attacker, Target, element, baseDamage, accuracy, crit, FX);
    }

    public void SpellAttackModified(GameObject Attacker, GameObject Target, Skills.MagElement type, int baseDamage, int accuracy, int crit, GameObject FX)
    {
        CombatManager cM = GameObject.Find("GameManager").GetComponent<CombatManager>();

        spellEfficacy = 1;
        spellLC = 1;
        difficulty = 1;

        spellPower = baseDamage;

        PartyStats attackerParty = Attacker.GetComponent<PartyStats>();
        EnemyStats attackerEnemy = Attacker.GetComponent<EnemyStats>();

        PartyStats targetParty = Target.GetComponent<PartyStats>();
        EnemyStats targetEnemy = Target.GetComponent<EnemyStats>();

        // Efficacy Checks
        bool isWeak = WeakCheck(type, Target);
        if (isWeak)
        {
            attackerParty.IncrementIP();

            // Extra Turn triggers when hitting a targets weakness with a specific element. It simply gives the player an extra turn as a reward for weakness exploitation
            cM.extraTurn = true;
            spellEfficacy = 1.5f; // Weakness increases damage by 50%
        }

        bool doesResist = ResCheck(type, Target);
        if (doesResist)
        {
            spellEfficacy = 0.5f; // Resistance decreases damage by 50%
        }

        // Level Correction Calculations

        spellSumFactor = AttackSumFactor(attackerLevel, defenderLevel);

        if (spellSumFactor == 0)
        {
            spellSumFactor = 1;
        }

        spellLC = AttackLevelCorrection(attackerLevel, defenderLevel);


        // Setting attack and defense stages based on whether attackers and targets are buffed or debuffed. Affects damage 
        // If Player is attacking
        if (attackerParty != null)
        {
            int TargetDefIndex = targetEnemy.DefenseStageIndex;
            int AttackerAtkIndex = attackerParty.AttackStageIndex;

            targetDefenseStage = targetEnemy.GetDefenseMultiplier(TargetDefIndex);
            attackerAttackStage = attackerParty.GetAttackMultiplier(AttackerAtkIndex);

            guardMod = 1f;
        }

        // If Enemy is attacking
        if (attackerEnemy != null)
        {
            int TargetDefIndex = targetParty.DefenseStageIndex;
            int AttackerAtkIndex = attackerEnemy.AttackStageIndex;

            targetDefenseStage = targetParty.GetDefenseMultiplier(TargetDefIndex);
            attackerAttackStage = attackerEnemy.GetAttackMultiplier(AttackerAtkIndex);

            if (targetParty.isGuarding)
            {
                guardMod = 0.8f;
            }
        }


        // Final Modified Damage
        spellMod = spellBase * (spellPower / 100) * spellEfficacy * spellLC * difficulty * targetDefenseStage * attackerAttackStage * guardMod * MagicCharge;



        // RNG Calculations
        float valA = 0;
        float valB = 0;

        spellMin = spellMod;

        valA = Random.Range(0, ((spellMin / 10) - 1));

        Mathf.Round(valA);

        if (valA <= 0)
            valA = 0;

        valB = Random.Range(0, 3);

        spellMax = spellMin + valA + valB;



        // Appying Damage After Checking Accuracy
        spellAccuracyRate = AccuracyCheck(Attacker, Target, accuracy);
        bool isHit = Random.Range(0f, 100f) < spellAccuracyRate;

        if (isHit)
        {
                float randomisedDmg = Random.Range(spellMin, spellMax);

                spellCritRate = CriticalCheck(Attacker, Target, crit);

                bool isCrit = Random.Range(0f, 100f) < spellCritRate;

            if (isCrit)
            {
                //Debug.Log("efficacy is = " + efficacy);
                // Crit only goes through as long as the enemy doesnt block or resist
                if (spellEfficacy >= 1)
                {
                    Debug.Log("!!!CRITICAL HIT!!!");

                    if (attackerParty != null)
                    {
                        targetEnemy.isCrit = true;
                    }

                    else if (attackerEnemy != null)
                    {
                        targetParty.isCrit = true;
                    }

                    // Critical strength multiplier
                    randomisedDmg *= 1.25f;
                    cM.extraTurn = true;

                }

            }

            int roundedDmg = Mathf.RoundToInt(randomisedDmg);

            if (attackerParty != null)
            {
                targetEnemy.DamageRetrieval(roundedDmg);
            }

            else if (attackerEnemy != null)
            {
                targetParty.DamageRetrieval(roundedDmg);
            }

            Debug.Log("Final Spell Damage is " + roundedDmg);

            StartCoroutine(SpawnSpellVFX(Attacker, Target, FX, type));
        }

        // If the accuracy check fails, the attacker misses and a UI popup is enabled indicating this
        else
        {
            if (attackerParty != null)
            {
                targetEnemy.hasMissed = true;
                targetEnemy.DamageRetrieval(0);
                Debug.Log("ATTACK MISSED!!");
            }

            if (attackerEnemy != null)
            {
                targetParty.hasMissed = true;
                targetParty.DamageRetrieval(0);
                Debug.Log("ATTACK MISSED!!");
            }

            cM.extraTurn = false;

        }


    }

    // Level Correction

    public float AttackSumFactor(int attackerLevel, int defenderLevel)
    {
        // Disables level scaling until players are around level 15 so they dont get wrongfully penalised for messing up early on. Should change based on balance
        if (attackerLevel + defenderLevel <= 30)
        {
            return 1;
        }

        else if (attackerLevel + defenderLevel > 30 && attackerLevel + defenderLevel <= 130)
        {
            return (attackerLevel + defenderLevel - 30) / 1000;
        }

        else if (attackerLevel + defenderLevel > 130)
        {
            return 0.1f;
        }

        return 1;
    }

    public float AttackLevelCorrection(int attackerLevel, int defenderLevel)
    {
        if (defenderLevel >= attackerLevel + 3)
        {
            float tempLC = 1 - Mathf.Sqrt(defenderLevel / attackerLevel - 1) * spellSumFactor * (defenderLevel - attackerLevel - 2);

            if (tempLC < 0.5)
            {
                tempLC = 0.5f;
            }

            return tempLC;
        }

        else if (attackerLevel >= defenderLevel + 3)
        {
            float tempLC = 1 + (Mathf.Sqrt(1 - (defenderLevel / attackerLevel)) * spellSumFactor * (attackerLevel - defenderLevel - 2) * 1.2f);

            if (tempLC > 1.5)
            {
                tempLC = 1.5f;
            }

            return tempLC;
        }

        return 1;
    }

    // Performing Checks

    public float CriticalCheck(GameObject Attacker, GameObject Target, int SpellCrit)
    {
        critLC = 1;

        PartyStats attackerParty = Attacker.GetComponent<PartyStats>();
        EnemyStats attackerEnemy = Attacker.GetComponent<EnemyStats>();

        PartyStats targetParty = Target.GetComponent<PartyStats>();
        EnemyStats targetEnemy = Target.GetComponent<EnemyStats>();

        if (attackerParty != null)
        {
            attackerLuck = attackerParty.mythosLuck;
            attackerLevel = attackerParty.playerLevel;

            targetLuck = targetEnemy.nightmareLuck;
            defenderLevel = targetEnemy.nightmareLevel;
        }
        else if (attackerEnemy != null)
        {
            attackerLuck = attackerEnemy.nightmareLuck;
            attackerLevel = attackerEnemy.nightmareLevel;

            targetLuck = targetParty.mythosLuck;
            defenderLevel = targetParty.playerLevel;
        }


        // Root Calculation
        critRoot = (attackerLuck + 10) / (targetLuck + 10);


        if (critRoot > 4.0f)
        {
            critRoot = 4.0f;
        }


        // Base Calculation
        critBase = ((attackerLuck + 10) / (targetLuck + 10) + (critRoot * critRoot)) * 3;
        //Debug.Log(critBase);

        // Level Correction
        if (attackerLevel < defenderLevel)
        {
            critLC = 1 - (Mathf.Sqrt(defenderLevel - attackerLevel) / 60 + (defenderLevel - attackerLevel) / 120);

            if (critLC < 0.5f)
                critLC = 0.5f;
        }

        // if attacker lvl is 1-10 higher
        else if (attackerLevel >= defenderLevel + 1 && attackerLevel <= defenderLevel + 10)
        {
            critLC = 1 + (attackerLevel - defenderLevel) * 0.025f;
        }

        // if attacker lvl is 11 higher
        else if (attackerLevel >= defenderLevel + 11)
        {
            critLC = 1.25f + (Mathf.Sqrt(attackerLevel - defenderLevel - 10) / 60 + (attackerLevel - defenderLevel - 10) / 120);

            if (critLC > 1.5f)
                critLC = 1.5f;
        }




        critRate = (SpellCrit + critBase) * critLC * difficulty;


        // Final Crit Rate Calculation

        if (critRate >= 100)
            critRate = 100;

        return critRate;


    }

    public float AccuracyCheck(GameObject Attacker, GameObject Target, int SpellAccuracy)
    {
        accuracyLC = 1;

        PartyStats attackerParty = Attacker.GetComponent<PartyStats>();
        EnemyStats attackerEnemy = Attacker.GetComponent<EnemyStats>();

        PartyStats targetParty = Target.GetComponent<PartyStats>();
        EnemyStats targetEnemy = Target.GetComponent<EnemyStats>();

        // If Player is attacking
        if (attackerParty != null)
        {
            attackerLuck = attackerParty.mythosLuck;
            attackerAgility = attackerParty.mythosAgility;

            targetLuck = targetEnemy.nightmareLuck;
            targetAgility = targetEnemy.nightmareAgility;

            attackerLevel = attackerParty.playerLevel;
            defenderLevel = targetEnemy.nightmareLevel;
        }

        // If Enemy is attacking
        else if (attackerEnemy != null)
        {
            attackerLuck = attackerEnemy.nightmareLuck;
            attackerAgility = attackerEnemy.nightmareAgility;

            targetLuck = targetParty.mythosLuck;
            targetAgility = targetParty.mythosAgility;

            attackerLevel = attackerEnemy.nightmareLevel;
            defenderLevel = targetParty.playerLevel;
        }


        // Root Calculation
        accuracyRoot = (attackerAgility * 3 + attackerLuck + 20) / (targetAgility * 3 + targetLuck + 20);
        Debug.Log(accuracyRoot);

        // Base Calculation
        if (accuracyRoot < 1)
        {
            accuracyBase = 1 - (Mathf.Sqrt(1 - accuracyRoot) / 30 + (1 - accuracyRoot) / 12);
            Debug.Log(accuracyBase);
        }

        else if (accuracyRoot >= 1)
        {
            accuracyBase = 1 + Mathf.Sqrt(accuracyRoot - 1) / 15 + (accuracyRoot - 1) * 2 / 3;
            Debug.Log(accuracyBase);
        }

        // Level Correction
        accuracyLC = ((attackerLevel + 10) / (defenderLevel + 10) - 1) / 5 + 1 + (attackerLevel - defenderLevel) / 200;
        Debug.Log(accuracyLC);

        if (accuracyLC > 1.2)
            accuracyLC = 1.2f;

        else if (accuracyLC < 1.0)
            accuracyLC = 1.0f;


        // Getiing Accuracy Value

        // If Player is attacking
        if (attackerParty != null)
        {
            int targetSpdIndex = targetEnemy.AgilityStageIndex;
            int attackerSpdIndex = attackerParty.AgilityStageIndex;

            targetSpeedStage = targetEnemy.GetAgilityMultiplier(targetSpdIndex);
            attackerSpeedStage = attackerParty.GetAgilityMultiplier(attackerSpdIndex);
        }
        // If Enemy is attacking
        else if (attackerEnemy != null)
        {
            int targetSpdIndex = targetParty.AgilityStageIndex;
            int attackerSpdIndex = attackerEnemy.AgilityStageIndex;

            targetSpeedStage = targetParty.GetAgilityMultiplier(targetSpdIndex);
            attackerSpeedStage = attackerEnemy.GetAgilityMultiplier(attackerSpdIndex);
        }


        accuracyMOD = accuracyBase * SpellAccuracy * accuracyLC * difficulty * targetSpeedStage * attackerSpeedStage;

         // Modified Calculation
        float accuracyScaled = 1;

        // Scaled Calculation
        if (accuracyMOD < 99)
        {
            accuracyScaled = accuracyMOD;
        }

        else if (accuracyMOD >= 99 && accuracyMOD < 114)
        {
            accuracyScaled = (accuracyMOD - 99) / 30 + 99;
        }

        else if (accuracyMOD >= 114 && accuracyMOD < 149)
        {
            accuracyScaled = (accuracyMOD - 114) / 70 + 99.5f;
        }

        else if (accuracyMOD >= 149)
        {
            accuracyScaled = 100;
        }

        accuracyRate = accuracyScaled;

        Debug.Log("Accuracy Rate is " + accuracyRate);

        return accuracyRate;

    }

    private bool WeakCheck(Skills.MagElement skillElement, GameObject Target)
    {
        if (Target.GetComponent<EnemyStats>() != null)
        {
            foreach (var item in Target.GetComponent<EnemyStats>().weakness)
            {
                if (item == skillElement)
                {
                    return true;
                }
            }
        }
        else if (Target.GetComponent<PartyStats>() != null)
        {
            foreach (var item in Target.GetComponent<PartyStats>().weakness)
            {
                if (item == skillElement)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private bool ResCheck(Skills.MagElement skillElement, GameObject Target)
    {
        if (Target.GetComponent<EnemyStats>() != null)
        {
            foreach (var item in Target.GetComponent<EnemyStats>().resist)
            {
                if (item == skillElement)
                {
                    return true;
                }
            }
        }
        else if (Target.GetComponent<PartyStats>() != null)
        {
            foreach (var item in Target.GetComponent<PartyStats>().resist)
            {
                if (item == skillElement)
                {
                    return true;
                }
            }
        }

        return false;
    }

}
