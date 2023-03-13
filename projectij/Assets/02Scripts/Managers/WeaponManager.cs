using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInfo
{
    public Define.Weapon weaponName;
    public int charIndex; // 이 무기를 가진 캐릭터 인덱스
    public int originalRarity; // 깃발 효과로 인해 3으로 변경되기 전의 원래 희귀도, 깃발 효과 적용할 때 말고 사용처 없음
    public int rarity; // 희귀도
    public Weapon weapon; // 실제 공격 무기
    public float coolTime; // 쿨타임
    public Dictionary<Define.CharacterStat, float> option = new Dictionary<Define.CharacterStat, float>(); // 랜덤으로 부여되는 옵션
    public int weaponIndex; // 중복 무기에서 몇 번째 무기인지
    public bool isStartWeapon; // 시작 무기인지
    public bool isUpgradedByFlag;
    public object Copy()
    {
        return this.MemberwiseClone();
    }
}

public class WeaponManager : MonoBehaviour
{
    // TODO: 아마 캐릭터 시작무기 추가되면 awake에서 따로 초기화해줘야할듯?
    //       게임 시작 전 캐릭터 효과로 인해 스탯이 조절되는 경우도 생각해야 함

    // 모든 무기 수치 정보 (데미지 제외) => csv로 받아오기
    private Dictionary<Define.Weapon, Weapon> allWeaponInfo = new Dictionary<Define.Weapon, Weapon>();
    // 유효/무효 스탯 정리 => csv로 받아오기
    private Dictionary<Define.Weapon, Dictionary<Define.CharacterStat, bool>> allWeaponBool = new Dictionary<Define.Weapon, Dictionary<Define.CharacterStat, bool>>();
    // 희귀도에 따른 무기 데미지 => csv로 받아오기
    private Dictionary<Define.Weapon, Dictionary<int, float>> allWeaponDamage = new Dictionary<Define.Weapon, Dictionary<int, float>>();
    // 플레이어가 가지고 있는 무기 정보 (무기별, 희귀도별 정리)
    public Dictionary<Define.Weapon, Dictionary<int, WeaponInfo[]>> currentWeaponInfo = new Dictionary<Define.Weapon, Dictionary<int, WeaponInfo[]>>();
    
    public Dictionary<Define.Weapon, Dictionary<Define.CharacterStat, float>> allWeaponOption = new Dictionary<Define.Weapon, Dictionary<Define.CharacterStat, float>>();

    // 캐릭터별 소유하고 있는 무기 개수
    public int[] charWeaponCount = { 0, 0, 0 };
    public int storageCount = 0;
    // 마스터리별 무기 개수
    public int[] masteryWeaponCount = { 0, 0, 0, 0 };
    // 시너지 관리
    public bool[] synergyOn = { false, false, false, false };
    private int[] synergyThreshold = { 3, 4, 4, 3 };
    // 둔기 시너지 관리용
    public int attackCount = 0;
    // 대포 시너지 관리용
    List<WeaponInfo> cannonInfos = new List<WeaponInfo>();

    List<Dictionary<string, object>> masteryDmgCSV;
    // 숙련도 이름, (숙련도 레벨, 데미지 증가량)
    Dictionary<Define.Mastery, Dictionary<int, float>> masteryDmg = new Dictionary<Define.Mastery, Dictionary<int, float>>();

    // 무기 옵션과 숙련도 효과로 추가되는 스탯 => (무기 변경, 숙련도 업 시 계산)
    public Dictionary<Define.CharacterStat, float> extraCharStat = new Dictionary<Define.CharacterStat, float>();

    float interval = 0.2f; // 연속 공격 사이의 시간 간격
    List<Dictionary<string, object>> weaponCSV;
    List<Dictionary<string, object>> damageCSV;
    List<Dictionary<string, object>> weaponBoolCSV; 
    List<List<string>> weaponOptionCSV;
    GameObject weaponInfoObj;
    GameObject currentWeaponInfoObj;

    public List<PlayerCharacters> playerCharacters = new List<PlayerCharacters>();

    // 희귀도4 검정무기
    private bool hasBlackWeapon = false;
    public bool HasBlackWeapon { get { return hasBlackWeapon; } }

    // 깃발 효과
    public int flagCount = 0;

    // 개인화기 시너지
    public bool personalWeaponSynergy = false;
    public Coroutine personalWeaponSynergyCoroutine = null;
    public IEnumerator personalWeaponSynergyOn()
    {
        personalWeaponSynergy = true;
        foreach (KeyValuePair<Define.Weapon, Dictionary<int, WeaponInfo[]>> kvp in currentWeaponInfo)
        {
            for (int i = 1; i < 5; i++)
            {
                if (kvp.Value[i] == null) continue; // 해당 희귀도의 무기 없음
                for (int j = 0; j < kvp.Value[i].Length; j++)
                {
                    if (kvp.Value[i][j] == null) continue;
                    Weapon currentWeapon = kvp.Value[i][j].weapon;
                    if (currentWeapon.IsChained) continue;
                    if (kvp.Value[i][j].charIndex == -1) continue;
                    if (currentWeapon.masteryName == Define.Mastery.개인화기 || currentWeapon.masteryName == Define.Mastery.대포)
                    {
                        kvp.Value[i][j].coolTime -= currentWeapon.CoolTime * 0.5f;
                    }
                }
            }
        }
        yield return new WaitForSeconds(10 + masteryWeaponCount[(int)Define.Mastery.개인화기] - 4);
        personalWeaponSynergy = false;
    }

    // StageManager 레벨업 할 때 호출
    // 개인화기 시너지
    public void CheckPersonalWeaponSynergy()
    {
        if (masteryWeaponCount[(int)Define.Mastery.개인화기] >= 4)
        {
            if (personalWeaponSynergyCoroutine != null)
                StopCoroutine(personalWeaponSynergyCoroutine);
            personalWeaponSynergyCoroutine = StartCoroutine(personalWeaponSynergyOn());
        }
    }    

    // 대포 시너지 추가 공격
    public void CannonSynergyAttack()
    {
        for (int i = 0; i < masteryWeaponCount[(int)Define.Mastery.대포] - 2; i++)
        {
            int rand = Random.Range(0, cannonInfos.Count);
            WeaponInfo weaponInfoCopy = (WeaponInfo)cannonInfos[rand].Copy();
            weaponInfoCopy.charIndex = 0;
            SpawnWeapon(weaponInfoCopy.weaponName, weaponInfoCopy);
        }
        StageManager.Instance.StartCoroutine(StageManager.Instance.ShakeCamera(1.5f, 0.7f));
    }

    private void Awake()
    {
        //  >>> 전체 무기 정보 받아오기 <<<
        weaponCSV = CSVReader.Read("CSV/AllWeaponInfo");
        damageCSV = CSVReader.Read("CSV/AllWeaponDamage");
        weaponBoolCSV = CSVReader.Read("CSV/allWeaponBool");
        TextAsset textAsset = Resources.Load("CSV/AllWeaponOption") as TextAsset;
        Debug.Log(textAsset.text);
        weaponOptionCSV = CSVReader.ReadByList(textAsset);
        weaponInfoObj = GameObject.Find("WeaponInfo");
        currentWeaponInfoObj = GameObject.Find("CurrentWeaponInfo");

        for (int i = 0; i < (int)Define.Weapon.upgradedWeaponEnd; i++)
        {
            if (i == (int)Define.Weapon.end) continue;
            // allWeaponInfo
            Weapon newWeapon = weaponInfoObj.AddComponent<Weapon>();

            allWeaponInfo.Add((Define.Weapon)i, newWeapon);

            allWeaponInfo[(Define.Weapon)i].weaponName = (Define.Weapon)i;
            allWeaponInfo[(Define.Weapon)i].upgradedWeaponName = GetUpgradedWeaponName((Define.Weapon)i);
            allWeaponInfo[(Define.Weapon)i].chainedWeaponName = GetChainedWeaponName((Define.Weapon)i);
            allWeaponInfo[(Define.Weapon)i].Size = float.Parse(weaponCSV[i]["Size"].ToString());
            allWeaponInfo[(Define.Weapon)i].Speed = float.Parse(weaponCSV[i]["Speed"].ToString());
            allWeaponInfo[(Define.Weapon)i].Force = float.Parse(weaponCSV[i]["Force"].ToString());
            allWeaponInfo[(Define.Weapon)i].KnockbackTime = float.Parse(weaponCSV[i]["KnockbackTime"].ToString());
            allWeaponInfo[(Define.Weapon)i].StunTime = float.Parse(weaponCSV[i]["StunTime"].ToString());
            allWeaponInfo[(Define.Weapon)i].CoolTime = float.Parse(weaponCSV[i]["CoolTime"].ToString());
            allWeaponInfo[(Define.Weapon)i].ProjectilePercentage = float.Parse(weaponCSV[i]["ProjectilePercentage"].ToString());
            allWeaponInfo[(Define.Weapon)i].PenetrationCount = int.Parse(weaponCSV[i]["PenetrationCount"].ToString());
            allWeaponInfo[(Define.Weapon)i].Duration = float.Parse(weaponCSV[i]["Duration"].ToString());
            allWeaponInfo[(Define.Weapon)i].Duration_Effect = float.Parse(weaponCSV[i]["Duration_Effect"].ToString());
            allWeaponInfo[(Define.Weapon)i].Duration_noAttack = float.Parse(weaponCSV[i]["Duration_noAttack"].ToString());
            allWeaponInfo[(Define.Weapon)i].IsChained = int.Parse(weaponCSV[i]["IsChained"].ToString()) == 1 ? true : false;
            allWeaponInfo[(Define.Weapon)i].ReducesCoolTime = int.Parse(weaponCSV[i]["ReducesCoolTime"].ToString()) == 1 ? true : false;
            allWeaponInfo[(Define.Weapon)i].masteryName = (Define.Mastery)int.Parse(weaponCSV[i]["Mastery"].ToString());

            // allWeaponBool
            allWeaponBool.Add(((Define.Weapon)i), new Dictionary<Define.CharacterStat, bool>());

            allWeaponBool[(Define.Weapon)i].Add(Define.CharacterStat.지속시간, int.Parse(weaponBoolCSV[i]["Duration"].ToString()) == 1 ? true : false);
            allWeaponBool[(Define.Weapon)i].Add(Define.CharacterStat.관통횟수, int.Parse(weaponBoolCSV[i]["PenetrationCount"].ToString()) == 1 ? true : false);
            allWeaponBool[(Define.Weapon)i].Add(Define.CharacterStat.쿨타임감소, int.Parse(weaponBoolCSV[i]["CoolTime"].ToString()) == 1 ? true : false);
            allWeaponBool[(Define.Weapon)i].Add(Define.CharacterStat.추가투사체확률, int.Parse(weaponBoolCSV[i]["ProjectilePercentage"].ToString()) == 1 ? true : false);
            allWeaponBool[(Define.Weapon)i].Add(Define.CharacterStat.넉백, int.Parse(weaponBoolCSV[i]["KnockBack"].ToString()) == 1 ? true : false);
            allWeaponBool[(Define.Weapon)i].Add(Define.CharacterStat.공격크기, int.Parse(weaponBoolCSV[i]["Size"].ToString()) == 1 ? true : false);

            // allWeaponOption
            allWeaponOption.Add(((Define.Weapon)i), new Dictionary<Define.CharacterStat, float>());

            for (int j = 0; j < (int)Define.CharacterStat.end; j++)
            {
                float value = float.Parse(weaponOptionCSV[i][j + 1].ToString());
                if (value != 0)
                    allWeaponOption[(Define.Weapon)i].Add((Define.CharacterStat)j, value);
            }
        }

        for (int i = 0; i < (int)Define.Weapon.end; i++)
        {
            allWeaponDamage.Add(((Define.Weapon)i), new Dictionary<int, float>());
            for (int j = 0; j < 4; j++)
            {
                allWeaponDamage[(Define.Weapon)i][j + 1] = float.Parse(damageCSV[i][(j + 1).ToString()].ToString());
            }
        }
        allWeaponDamage.Add(Define.Weapon.각궁_불화살연계, new Dictionary<int, float>());
        for (int j = 0; j < 4; j++)
        {
            allWeaponDamage[Define.Weapon.각궁_불화살연계][j + 1] = float.Parse(damageCSV[20][(j + 1).ToString()].ToString());
        }
        allWeaponDamage.Add(Define.Weapon.석궁_불화살연계, new Dictionary<int, float>());
        for (int j = 0; j < 4; j++)
        {
            allWeaponDamage[Define.Weapon.석궁_불화살연계][j + 1] = float.Parse(damageCSV[21][(j + 1).ToString()].ToString());
        }
        allWeaponDamage.Add(Define.Weapon.현자총통_비격진천뢰연계, new Dictionary<int, float>());
        for (int j = 0; j < 4; j++)
        {
            allWeaponDamage[Define.Weapon.현자총통_비격진천뢰연계][j + 1] = float.Parse(damageCSV[21][(j + 1).ToString()].ToString());
        }

        // >>> 숙련도별 데미지 증가량 받아오기 <<<
        masteryDmgCSV = CSVReader.Read("CSV/DmgPerMastery");
        for (int i = 0; i < (int)Define.Mastery.end; i++)
        {
            masteryDmg.Add((Define.Mastery)i, new Dictionary<int, float>());
            // 숙련도 레벨별 데미지 받아오기
            for (int j = 1; j < 11; j++)
            {
                masteryDmg[(Define.Mastery)i].Add(j, float.Parse(masteryDmgCSV[i][j.ToString()].ToString()));
            }
        }
        playerCharacters.Add(StageManager.Instance.player.GetComponent<Player>());
        for(int i = 0; i < StageManager.Instance.followers.Count; i++)
        {
            playerCharacters.Add(StageManager.Instance.followers[i].GetComponent<follower>());
        }

        // extraCharStat 초기화
        for (int i = 0; i < (int)Define.CharacterStat.end; i++)
        {
            extraCharStat.Add((Define.CharacterStat)i, 0);
        }


        // for (int i = 0; i < 3; i++)
        // {
        //     coolTimeSlider[i] = playerCharacters[i].transform.GetChild(2).GetChild(0).GetComponent<Slider>();
        // }
    }

    // 쿨타임 슬라이더 관련
    WeaponInfo[] minCoolTimeWeaponInfo = new WeaponInfo[3];
    float[] maxValue = { 0, 0, 0 };
    float[] minValue = { 100, 100, 100 };

    public void Fixed_Update()
    {
        foreach (KeyValuePair<Define.Weapon, Dictionary<int, WeaponInfo[]>> kvp in currentWeaponInfo)
        {
            for (int i = 1; i < 5; i++)
            {
                if (kvp.Value[i] == null) continue; // 해당 희귀도의 무기 없음
                for (int j = 0; j < kvp.Value[i].Length; j++)
                {
                    if (kvp.Value[i][j] == null) continue;
                    WeaponInfo currentWeaponInfo = kvp.Value[i][j];
                    Weapon currentWeapon = kvp.Value[i][j].weapon;
                    if (currentWeapon.IsChained) continue;
                    if (currentWeaponInfo.charIndex == -1) continue;

                    // 공격(무기생성)
                    StartCoroutine(Attack(currentWeaponInfo));
                }
            }
        }
    }

    // 새로운 무기 획득 시 AddWeapon 전에 호출
    public Dictionary<Define.CharacterStat, float> GetWeaponOption(Define.Weapon weaponName, int rarity)
    {
        if (rarity == 1) return null;

        Dictionary<Define.CharacterStat, float> totalOption = new Dictionary<Define.CharacterStat, float>();
        if ((int)weaponName > (int)Define.Weapon.end)
            weaponName = GetOriginalWeaponName(weaponName);
        foreach(KeyValuePair<Define.CharacterStat, float> option in allWeaponOption[weaponName])
        {
            totalOption.Add(option.Key, option.Value * Mathf.Pow(3, rarity - 2));
        }
        return totalOption;
    }

    int SetWeaponIndex(Define.Weapon weaponName, int rarity)
    {
        int i = 0;
        bool flag = false;
        // 최대 인덱스: 4
        while (i < 5)
        {
            flag = false;
            foreach (WeaponInfo weaponInfo in currentWeaponInfo[weaponName][rarity])
            {
                if (weaponInfo == null) continue;
                if (i == weaponInfo.weaponIndex)
                {
                    i++;
                    flag = true;
                    break;
                }
            }
            if (flag == false) break;
        }
        return i;
    }

    // 외부(게임매니저)에서 호출
    // index 0: player / 1: follower1 / 2: follower2
    public WeaponInfo AddWeapon(Define.Weapon weaponName, int charIndex, int rarity, Dictionary<Define.CharacterStat, float> option, bool addIcon, bool isStartWeapon)
    {
        // 검정무기
        if (rarity == 4)
        {
            hasBlackWeapon = true;
            for (int i = 0; i < 3; i++)
            {
                if (charIndex == i) 
                    playerCharacters[i].HasBlackWeapon = true;
                else
                    playerCharacters[i].HasBlackWeapon = false;
            }
        }

        string tempString = $"{weaponName} / charIndex: {charIndex} / 옵션: ";
        if (option != null)
        {
            foreach (KeyValuePair<Define.CharacterStat, float> optionKvp in option)
            {
                tempString += $"{optionKvp.Key} : +{optionKvp.Value} / ";
            }
            Debug.Log(tempString);
        }

        // 희귀도 3이면 진화무기 생성
        if (rarity >= 3)
        {
            Define.Weapon upgradedWeaponName = GetUpgradedWeaponName(weaponName);
            if (GetUpgradedWeaponName(weaponName) != Define.Weapon.end)
                weaponName = upgradedWeaponName;
            
            // 시작무기 false로 변경
            if (isStartWeapon)
            {
                isStartWeapon = false;
                StageManager.Instance.SetStartingWeaponUI();
            }
        }

        // 무기 종류 처음 획득
        if (currentWeaponInfo.ContainsKey(weaponName) == false)
        {
            ObjectPoolManager.Instance.CreateNewWeapons(weaponName);
            currentWeaponInfo.Add(weaponName, new Dictionary<int, WeaponInfo[]>());
            for (int i = 1; i < 5; i++)
                currentWeaponInfo[weaponName].Add(i, new WeaponInfo[5]); // 희귀도 4개 모두 빈 배열 생성 (최대 5개. 필요하면 추가)
        }

        // 무기 정보 복사해서 currentWeaponInfo에 추가
        Weapon weaponCopy = currentWeaponInfoObj.AddComponent<Weapon>(); // 무기 원본 정보
        weaponCopy.weaponName = weaponName;
        CopyWeapon(weaponCopy, rarity); // allWeaponInfo에 저장된 무기 정보 복사

        WeaponInfo newWeaponInfo = new WeaponInfo();
        newWeaponInfo.weaponName = weaponName;
        newWeaponInfo.charIndex = charIndex;
        newWeaponInfo.originalRarity = rarity;
        newWeaponInfo.rarity = rarity;
        newWeaponInfo.weapon = weaponCopy;
        newWeaponInfo.coolTime = weaponCopy.CoolTime;
        newWeaponInfo.weaponIndex = SetWeaponIndex(weaponName, rarity);
        newWeaponInfo.option = option;
        newWeaponInfo.isStartWeapon = isStartWeapon;

        currentWeaponInfo[weaponName][rarity][newWeaponInfo.weaponIndex] = newWeaponInfo;
        
        if (newWeaponInfo.weapon.IsChained == false)
        {
            // 아이콘 추가
            if (addIcon)
                StageManager.Instance.AddWeaponIcon(charIndex, newWeaponInfo);

            // 캐릭터인덱스 확인 => 옵션 적용
            if (charIndex > -1)
            {
                AddOption(newWeaponInfo);
                StageManager.Instance.ColorIcon(charIndex);
                charWeaponCount[charIndex]++;
            }
            else
                storageCount++;

            // 대포 정보 보관
            if (newWeaponInfo.weapon.masteryName == Define.Mastery.대포)
                cannonInfos.Add(newWeaponInfo);

            // 숙련도별 무기 개수
            if (newWeaponInfo.weapon.masteryName != Define.Mastery.방어구)
                masteryWeaponCount[(int)weaponCopy.masteryName]++;
            // 방어구라면 패시브 온
            else
            {
                ObjectPoolManager.Instance.GetWeaponClass(weaponName).PassiveOn();
            }
        }

        // 연계무기 추가
        if (newWeaponInfo.weapon.chainedWeaponName != Define.Weapon.end)
        {
            AddWeapon(newWeaponInfo.weapon.chainedWeaponName, charIndex, rarity, option, addIcon, isStartWeapon);
        }

        tempString = "현재 무기 정보 >>> ";
        foreach (KeyValuePair<Define.Weapon, Dictionary<int, WeaponInfo[]>> kvp in currentWeaponInfo)
        {
            for (int i = 1; i < 5; i++)
            {
                if (kvp.Value[i] == null) continue; // 해당 희귀도의 무기 없음
                for (int j = 0; j < kvp.Value[i].Length; j++)
                {
                    if (kvp.Value[i][j] == null) continue;
                    Weapon currentWeapon = kvp.Value[i][j].weapon;
                    if (currentWeapon.IsChained) continue;
                    tempString += $"{currentWeapon.weaponName} {kvp.Value[i][j].charIndex}의 {kvp.Value[i][j].weaponIndex}/ ";
                }
            }
        }
        Debug.Log(tempString);

        // 시너지 확인
        for (int i = 0; i < 4; i++)
        {
            if(masteryWeaponCount[i] >= synergyThreshold[i])
                synergyOn[i] = true;
            else
                synergyOn[i] = false;
        }
        // 둔기 시너지 켜짐
        if (masteryWeaponCount[1] == synergyThreshold[1])
        {
            // 공격횟수 초기화
            attackCount = 0;            
        }
        
        // Debug.Log($"{masteryWeaponCount[0]} {masteryWeaponCount[1]} {masteryWeaponCount[2]} {masteryWeaponCount[3]}");

        StageManager.Instance.SetSynergyIcon(synergyOn);

        return newWeaponInfo;
    }

    // 무기 진화 시 원래 무기 삭제 용
    public void DeleteWeapon(WeaponInfo weaponInfo, bool deleteIcon)
    {
        //Debug.Log($"삭제 전 : {weaponInfo.coolTime}");
        Component[] components = currentWeaponInfoObj.GetComponents(typeof(Weapon));
        foreach (Component component in components)
        {
            if (component == weaponInfo.weapon)
            {
                Destroy(component);
                break;
            }
        }

        // 연계무기 삭제
        if (weaponInfo.weapon.chainedWeaponName != Define.Weapon.end)
        {
            DeleteWeapon(currentWeaponInfo[weaponInfo.weapon.chainedWeaponName][weaponInfo.rarity][weaponInfo.weaponIndex], false);
        }
        // 무기 삭제
        //currentWeaponInfo[weaponInfo.weapon.weaponName][weaponInfo.rarity][weaponInfo.weaponIndex].coolTime -= 2;
        currentWeaponInfo[weaponInfo.weapon.weaponName][weaponInfo.rarity][weaponInfo.weaponIndex] = null;

        Debug.Log($"{weaponInfo.weapon.weaponName} {weaponInfo.rarity} 삭제 완료");

        //Debug.Log($"삭제 후 : {weaponInfo.coolTime}");

        if (deleteIcon)
            StageManager.Instance.DeleteWeaponIcon(weaponInfo);

        if (weaponInfo.weapon.IsChained == false)
        {
            if (weaponInfo.charIndex > -1)
            {
                charWeaponCount[weaponInfo.charIndex]--;
                StageManager.Instance.ColorIcon(weaponInfo.charIndex);
                // 무기 옵션 제거
                DeleteOption(weaponInfo);
            }
            else
            {
                storageCount--;
            }
            if (weaponInfo.weapon.masteryName == Define.Mastery.대포)
                cannonInfos.Remove(weaponInfo);

            if (weaponInfo.weapon.masteryName != Define.Mastery.방어구)
                masteryWeaponCount[(int)weaponInfo.weapon.masteryName]--;
            else
            {
                ObjectPoolManager.Instance.GetWeaponClass(weaponInfo.weapon.weaponName).PassiveOff();
            }
        }
        
    }

    public void SwapWeapon(int firstCharIndex, int secondCharIndex, WeaponInfo weaponInfo1, WeaponInfo weaponInfo2)
    {
        
        if (weaponInfo1 == null) return; // 처음 선택한 곳이 빈 칸이면 스왑 무시
        if (weaponInfo1.isStartWeapon) return; // 시작무기 스왑 무시
        if (weaponInfo2 == null) // 나중에 선택한 곳이 빈 칸이면 그 캐릭터에 무기 추가
        {
            // Debug.Log("무기 다른 애한테 줌!");
            DeleteWeapon(CheckIfWeaponExists(weaponInfo1), true);
            AddWeapon(weaponInfo1.weapon.weaponName, secondCharIndex, weaponInfo1.rarity, weaponInfo1.option, true, false);
        }
        else // 나중에 선택한 곳에 무기가 있다면 스왑
        {
            if (weaponInfo2.isStartWeapon) return; // 시작무기 스왑 무시
            //Debug.Log("무기 스왑!");
            //Debug.Log($"{firstCharIndex}: {StageManager.Instance.firstClickedInfo.Item2} / {secondCharIndex}: {StageManager.Instance.secondClickedInfo.Item2}");
            DeleteWeapon(CheckIfWeaponExists(weaponInfo1), false);
            DeleteWeapon(CheckIfWeaponExists(weaponInfo2), false);

            // 이미지 인포에 변경된 인덱스가 반영되도록
            // Debug.Log($"{weaponInfo1.weapon.weaponName} {secondCharIndex} {weaponInfo1.rarity}");
            StageManager.Instance.firstClickedInfo.Item3 = 
                AddWeapon(weaponInfo1.weapon.weaponName, secondCharIndex, weaponInfo1.rarity, weaponInfo1.option, false, false);
            StageManager.Instance.secondClickedInfo.Item3 = 
                AddWeapon(weaponInfo2.weapon.weaponName, firstCharIndex, weaponInfo2.rarity, weaponInfo2.option, false, false);

            StageManager.Instance.SwapIcon();
            if (firstCharIndex > -1)
                StageManager.Instance.ColorIcon(firstCharIndex);
            if (secondCharIndex > -1)
                StageManager.Instance.ColorIcon(secondCharIndex);
        }
    }

    public WeaponInfo CheckIfWeaponExists(WeaponInfo newWeaponInfo)
    {
        WeaponInfo weaponInfo = currentWeaponInfo[newWeaponInfo.weapon.weaponName][newWeaponInfo.rarity][newWeaponInfo.weaponIndex];

        return weaponInfo;
    }

    // 공격 (무기 생성) 코루틴
    IEnumerator Attack(WeaponInfo weaponInfo)
    {
        if (0 <= weaponInfo.coolTime)
        {
            // 승자총통 => 정지 시에만 쿨타임 돎
            if (weaponInfo.weaponName == Define.Weapon.승자총통 || weaponInfo.weaponName == Define.Weapon.승자총통_삼연자포)
            {
                if (StageManager.Instance.inputDir != (0, 0)) yield break;
            }
            // 왜검 => 이동 시에만 쿨타임 돎
            else if (weaponInfo.weaponName == Define.Weapon.왜검 || weaponInfo.weaponName == Define.Weapon.야태도)
            {
                if (StageManager.Instance.inputDir == (0, 0)) yield break;
            }
            // 일부 무기 정지 시 쿨타임 빨리 돎
            if (weaponInfo.weapon.ReducesCoolTime && StageManager.Instance.inputDir == (0, 0))
                weaponInfo.coolTime -= Time.fixedDeltaTime * 1.2f;
            // 해당 없으면 정상적으로 쿨타임 감소
            else
                weaponInfo.coolTime -= Time.fixedDeltaTime;
            yield break;
        }

        // 깃발 효과
        if (flagCount > 0 && weaponInfo.rarity < 3 && GetUpgradedWeaponName(weaponInfo.weaponName) != Define.Weapon.end)
        {
            flagCount--;
            weaponInfo.isUpgradedByFlag = true;
        }
        else
            weaponInfo.isUpgradedByFlag = false;

        Weapon currentWeapon = SpawnWeapon(weaponInfo.weaponName, weaponInfo);

        // 둔기 시너지
        if (synergyOn[1])
        {
            attackCount++;
            if (attackCount >= 20 - (masteryWeaponCount[(int)Define.Mastery.둔기] - 4))
            {
                currentWeapon.enemyDeath = true;
                attackCount = 0;
            }
            else
                currentWeapon.enemyDeath = false;
        }

        // 개인화기 시너지 => 개인화기와 대포 쿨타임 감소
        if (personalWeaponSynergy && (weaponInfo.weapon.masteryName == Define.Mastery.개인화기 || weaponInfo.weapon.masteryName == Define.Mastery.대포))
        {
            weaponInfo.coolTime = currentWeapon.CoolTime * 0.5f;
        }
        else
            weaponInfo.coolTime = currentWeapon.CoolTime;

        // 대포 공격 시 카메라 쉐이킹
        if (weaponInfo.weapon.masteryName == Define.Mastery.대포)
        {
            StageManager.Instance.StartCoroutine(StageManager.Instance.ShakeCamera(1.5f, 0.7f));
        }
        currentWeapon.isFirstWeapon = true;
        //Debug.Log($" next: {nextAttackTimeArray[index][weapon.weaponName]}초");

        // 숙련도 경험치 추가 (일단은 공격 당 10으로)
        // 특정 조건이 아니면 공격하지 않는 무기들 제외 => 얘네는 따로 추가해줌
        // (조총, 환도, 왜검)
        Define.Weapon weaponName = GetOriginalWeaponName(weaponInfo.weaponName);
        if (weaponName != Define.Weapon.조총 && weaponName != Define.Weapon.환도 && weaponName != Define.Weapon.왜검 && weaponInfo.weapon.masteryName != Define.Mastery.방어구)
        {
            playerCharacters[weaponInfo.charIndex].GainMasteryExp(currentWeapon.masteryName, 10);
        }

        // 무기 연속 생성 방식이 다른 무기들 => 첫 무기 생성 후 반영
        if (weaponName == Define.Weapon.승자총통 || weaponName == Define.Weapon.현자총통 || weaponName == Define.Weapon.왜검) yield break;

        // 무기 연속 생성
        for (int i = 0; i < currentWeapon.ProjectilePercentage - 1; i++)
        {
            yield return new WaitForSeconds(interval);
            Weapon extraWeapon = SpawnWeapon(weaponInfo.weaponName, weaponInfo);
            extraWeapon.isFirstWeapon = true;
            extraWeapon.enemyDeath = currentWeapon.enemyDeath;
        }

        yield return null;
    }

    // Attack에서 호출되는 함수
    public Weapon SpawnWeapon(Define.Weapon weaponName, WeaponInfo weaponInfo)
    {
        Define.Weapon originalWeaponName = weaponName;
        weaponInfo.weaponName = weaponName;

        // SpawnWeapon이 내부에서 호출되었다면 weaponInfo는 currentWeaponInfo의 무기 정보 / weaponName은 weaponInfo.weaponName
        // SpawnWeapon이 외부에서 호출되었다면 weaponInfo는 연계무기의 weaponInfoCopy / weaponName은 업그레이드된 무기 이름
        if (weaponInfo.isUpgradedByFlag)
        {
            originalWeaponName = GetOriginalWeaponName(weaponName);
            
            if (originalWeaponName != weaponName)
            {
                weaponInfo = currentWeaponInfo[originalWeaponName][weaponInfo.originalRarity][weaponInfo.weaponIndex];
            } 
            // else: 예)각궁불화살연계 => 각궁불화살의 weaponInfoCopy 사용 (복사본이라 currentWeaponInfo에 영향 x)
            
            // 잠시 currentWeaponInfo의 weaponInfo의 무기이름과 희귀도 변경
            weaponInfo.rarity = 3;
            if (GetUpgradedWeaponName(weaponName) != Define.Weapon.end)
            {
                weaponInfo.weaponName = GetUpgradedWeaponName(weaponName);
                weaponName = weaponInfo.weaponName;
            } 
            // else: 외부에서 호출되어 이미 weaponName이 진화된 버전인 경우
            Debug.Log($"진화된 공격! :{weaponName}");
        }

        GameObject obj = ObjectPoolManager.Instance.GetWeapon(weaponName); // 풀에서 무기 가져오기
        Weapon newWeapon = obj.GetComponent<Weapon>();

        UpdateWeapon(weaponInfo, newWeapon); // 수치 업데이트

        // 깃발효과로 변경된 currentWeaponInfo의 weaponInfo 원래대로 되돌리기
        if (weaponInfo.isUpgradedByFlag)
        {
            weaponInfo.weaponName = originalWeaponName;
            weaponInfo.rarity = weaponInfo.originalRarity;
            weaponInfo.isUpgradedByFlag = false;
        }

        Transform playerTrans = playerCharacters[weaponInfo.charIndex].transform;
        newWeapon.SetWeapon(weaponInfo, playerTrans, playerTrans.position); // 수치 적용

        return newWeapon;
    }

    // 무기 초기화 / 무기 처음 획득 시 호출
    public void CopyWeapon(Weapon weapon, int rarity)
    {
        Weapon initialWeapon = allWeaponInfo[weapon.weaponName];

        weapon.Size = initialWeapon.Size;
        weapon.Speed = initialWeapon.Speed;
        weapon.StunTime = initialWeapon.StunTime;
        weapon.KnockbackTime = initialWeapon.KnockbackTime;
        weapon.Force = initialWeapon.Force;
        weapon.CoolTime = initialWeapon.CoolTime;
        weapon.ProjectilePercentage = initialWeapon.ProjectilePercentage;
        weapon.PenetrationCount = initialWeapon.PenetrationCount;
        weapon.Duration = initialWeapon.Duration;
        weapon.Duration_Effect = initialWeapon.Duration_Effect;
        weapon.IsChained = initialWeapon.IsChained;
        weapon.ReducesCoolTime = initialWeapon.ReducesCoolTime;
        weapon.masteryName = initialWeapon.masteryName;

        weapon.upgradedWeaponName = initialWeapon.upgradedWeaponName;
        weapon.chainedWeaponName = initialWeapon.chainedWeaponName;

        if ((int) weapon.weaponName < (int) Define.Weapon.end)
            weapon.Damage = allWeaponDamage[weapon.weaponName][rarity];
        else
            weapon.Damage = allWeaponDamage[GetOriginalWeaponName(weapon.weaponName)][rarity];
    }

    // 무기 생성마다 호출
    public void UpdateWeapon(WeaponInfo weaponInfo, Weapon weapon)
    {
        PlayerCharacters character = playerCharacters[weaponInfo.charIndex];

        Weapon baseWeapon = allWeaponInfo[weaponInfo.weaponName];

        if ((int)weaponInfo.weaponName < (int)Define.Weapon.end)
            baseWeapon.Damage = allWeaponDamage[weaponInfo.weaponName][weaponInfo.rarity];
        else
            baseWeapon.Damage = allWeaponDamage[GetOriginalWeaponName(weaponInfo.weaponName)][weaponInfo.rarity];

        // 무기 사용하는 캐릭터의 스탯
        Dictionary<Define.CharacterStat, float> tempStat = character.GetCharacterStat();
        //퀘스트로 얻은 스텟
        Dictionary<Define.CharacterStat, float> quest_tempStat = GameManager.gameManager_Instance.squad[weaponInfo.charIndex].quest_stat;

        if (allWeaponBool[baseWeapon.weaponName][Define.CharacterStat.공격크기])
            weapon.Size = baseWeapon.Size * tempStat[Define.CharacterStat.공격크기];
        else
            weapon.Size = baseWeapon.Size;

        weapon.Speed = baseWeapon.Speed * tempStat[Define.CharacterStat.투사체속도];
        weapon.StunTime = baseWeapon.StunTime;

        if (allWeaponBool[baseWeapon.weaponName][Define.CharacterStat.넉백])
        {
            weapon.KnockbackTime = baseWeapon.KnockbackTime;
            weapon.Force = baseWeapon.Force * tempStat[Define.CharacterStat.넉백];
        }
        else
        {
            weapon.KnockbackTime = 0;
            weapon.Force = 0;
        }
        // 숙련도에 따른 추가 데미지
        int masteryLevel = character.GetMasteryLevel(baseWeapon.masteryName);
        weapon.Damage = baseWeapon.Damage * (tempStat[Define.CharacterStat.데미지] + masteryDmg[baseWeapon.masteryName][masteryLevel]);

        if (allWeaponBool[baseWeapon.weaponName][Define.CharacterStat.쿨타임감소])
            weapon.CoolTime = baseWeapon.CoolTime * (1 - tempStat[Define.CharacterStat.쿨타임감소]);
        else
            weapon.CoolTime = baseWeapon.CoolTime;

        if (allWeaponBool[baseWeapon.weaponName][Define.CharacterStat.추가투사체확률])
        {
            if (baseWeapon.masteryName == Define.Mastery.개인화기)
                weapon.ProjectilePercentage = baseWeapon.ProjectilePercentage + tempStat[Define.CharacterStat.추가투사체확률] * 2;
            else
                weapon.ProjectilePercentage = baseWeapon.ProjectilePercentage + tempStat[Define.CharacterStat.추가투사체확률];
        }
        else
            weapon.ProjectilePercentage = baseWeapon.ProjectilePercentage;

        // 캐릭터가 검정무기를 갖고 있다면 관통횟수 무한(인 척 500 넣기)
        if (hasBlackWeapon && playerCharacters[weaponInfo.charIndex].HasBlackWeapon)
        {
            weapon.PenetrationCount = 500;
        }
        else
        {
            if (allWeaponBool[baseWeapon.weaponName][Define.CharacterStat.관통횟수])
                weapon.PenetrationCount = baseWeapon.PenetrationCount + (int)tempStat[Define.CharacterStat.관통횟수];
            else
                weapon.PenetrationCount = baseWeapon.PenetrationCount;
        }
        
        if (allWeaponBool[baseWeapon.weaponName][Define.CharacterStat.지속시간])
            weapon.Duration = baseWeapon.Duration * tempStat[Define.CharacterStat.지속시간];
        else
            weapon.Duration = baseWeapon.Duration;

        weapon.Duration_Effect = baseWeapon.Duration_Effect;
        weapon.IsChained = baseWeapon.IsChained;
        weapon.masteryName = baseWeapon.masteryName;

        weapon.upgradedWeaponName = baseWeapon.upgradedWeaponName;
        weapon.chainedWeaponName = baseWeapon.chainedWeaponName;
        weapon.weaponInfoCopy.weaponName = weaponInfo.weaponName;
        weapon.weaponInfoCopy.charIndex = weaponInfo.charIndex;
        weapon.weaponInfoCopy.originalRarity = weaponInfo.originalRarity;
        weapon.weaponInfoCopy.rarity = weaponInfo.rarity;
        weapon.weaponInfoCopy.weapon = weaponInfo.weapon;
        weapon.weaponInfoCopy.coolTime = weaponInfo.coolTime;
        weapon.weaponInfoCopy.option = weaponInfo.option;
        weapon.weaponInfoCopy.weaponIndex = weaponInfo.weaponIndex;
        weapon.weaponInfoCopy.isStartWeapon = weaponInfo.isStartWeapon;
        weapon.weaponInfoCopy.isUpgradedByFlag = weaponInfo.isUpgradedByFlag;
    }

    // 테스트 용으로만 사용
    public void CalculateTotalOption()
    {
        // tempExtraCharStat 초기화
        Dictionary<Define.CharacterStat, float> tempExtraCharStat = new Dictionary<Define.CharacterStat, float>();
        for (int i = 0; i < (int)Define.CharacterStat.end; i++)
        {
            tempExtraCharStat.Add((Define.CharacterStat)i, 0);
        }

        // tempExtraCharStat 계산
        foreach (KeyValuePair<Define.Weapon, Dictionary<int, WeaponInfo[]>> kvp in currentWeaponInfo)
        {
            int charIndex;
            for (int i = 1; i < 5; i++)
            {
                if (kvp.Value[i] == null) continue; // 해당 희귀도의 무기 없음
                for (int j = 0; j < kvp.Value[i].Length; j++)
                {
                    WeaponInfo currentWeaponInfo = kvp.Value[i][j];
                    if (currentWeaponInfo.weapon.IsChained) continue; // 연계무기 옵션 무시
                    charIndex = currentWeaponInfo.charIndex;
                    Define.Mastery masteryName = currentWeaponInfo.weapon.masteryName;
                }
            }
        }

        // extraCharStat 에 복사
        string tempString = "추가 옵션 >>> ";
        for (int i = 0; i < (int)Define.CharacterStat.end; i++)
        {
            //extraCharStat[(Define.CharacterStat)i] = tempExtraCharStat[(Define.CharacterStat)i];
            if (tempExtraCharStat[(Define.CharacterStat)i] != 0)
            {
                tempString += $"{(Define.CharacterStat)i} : +{tempExtraCharStat[(Define.CharacterStat)i]} / ";
            }
        }
        Debug.Log(tempString);
    }

    protected void SetCharacterStat()
    {
        if (extraCharStat[Define.CharacterStat.쿨타임감소] >= 0.66f)
            extraCharStat[Define.CharacterStat.쿨타임감소] = 0.66f;

        // 스탯 적용
        foreach(PlayerCharacters playerCharacters in playerCharacters)
        {
            playerCharacters.SetCharacterStat(extraCharStat);
        }

        StageManager.Instance.UpdateStatText();
    }

    // 무기 추가 시 호출 (무기 추가 / 무기 진화 / 무기 교환)
    public void AddOption(WeaponInfo weaponInfo)
    {
        if (weaponInfo.weapon.IsChained) return; // 연계무기 옵션 무시
        if (weaponInfo.charIndex == -1) return;
        if (StageManager.Instance.isWounded[weaponInfo.charIndex]) return; // 부상입은 캐릭터 무기 옵션 무시
        if (weaponInfo.option != null)
        {
            foreach (KeyValuePair<Define.CharacterStat, float> option in weaponInfo.option)
            {
                extraCharStat[option.Key] += option.Value;
            }
            SetCharacterStat();
            PrintTotalOption();
        }
        //Debug.Log($"charIndex: {charIndex} / masteryLv: {playerCharacters[charIndex].GetMasteryLevel(masteryName) - 1}");
    }

    // 무기 삭제 시 호출 (무기 진화 / 무기 교환)
    public void DeleteOption(WeaponInfo weaponInfo)
    {
        if (weaponInfo.weapon.IsChained) return; // 연계무기 옵션 무시
        if (weaponInfo.charIndex == -1) return;
        if (StageManager.Instance.isWounded[weaponInfo.charIndex]) return; // 부상입은 캐릭터 무기 옵션 무시
        if (weaponInfo.option == null) return;

        foreach (KeyValuePair<Define.CharacterStat, float> option in weaponInfo.option)
        {
            extraCharStat[option.Key] -= option.Value;
        }      
        SetCharacterStat();
        //PrintTotalOption();
    }


    // 숙련도 증가 시 호출
    public void UpgradeOption(Define.Character character, Define.Mastery mastery)
    {
        foreach (KeyValuePair<Define.Weapon, Dictionary<int, WeaponInfo[]>> kvp in currentWeaponInfo)
        {
            for (int i = 1; i < 5; i++)
            {
                if (kvp.Value[i] == null) continue; // 해당 희귀도의 무기 없음
                for (int j = 0; j < kvp.Value[i].Length; j++)
                {
                    WeaponInfo currentWeaponInfo = kvp.Value[i][j];
                    if (currentWeaponInfo == null) continue;
                    if (currentWeaponInfo.weapon.masteryName == mastery && currentWeaponInfo.charIndex == (int)character)
                    {
                        if (currentWeaponInfo.weapon.IsChained) continue; // 연계무기 옵션 무시
                        if (currentWeaponInfo.option == null) continue;
                        foreach (KeyValuePair<Define.CharacterStat, float> optionKvp in currentWeaponInfo.option)
                        {
                            // 일단은 숙련도 1마다 10%씩 추가 스탯
                            extraCharStat[optionKvp.Key] += optionKvp.Value * 0.1f;
                        }
                    }                   
                }
            }
        }
        SetCharacterStat();
        PrintTotalOption();
    }

    // 캐릭터 부상 상태 시 호출
    public void DowngradeOption(Define.Character character)
    {
        foreach (KeyValuePair<Define.Weapon, Dictionary<int, WeaponInfo[]>> kvp in currentWeaponInfo)
        {
            for (int i = 1; i < 5; i++)
            {
                if (kvp.Value[i] == null) continue; // 해당 희귀도의 무기 없음
                for (int j = 0; j < kvp.Value[i].Length; j++)
                {
                    WeaponInfo currentWeaponInfo = kvp.Value[i][j];
                    if (currentWeaponInfo == null) continue;
                    if (currentWeaponInfo.charIndex == (int)character)
                    {
                        if (currentWeaponInfo.weapon.IsChained) continue; // 연계무기 옵션 무시
                        if (currentWeaponInfo.option == null) continue;
                        foreach (KeyValuePair<Define.CharacterStat, float> optionKvp in currentWeaponInfo.option)
                        {
                            extraCharStat[optionKvp.Key] -= optionKvp.Value;
                            // 0에 가까운 수치는 그냥 0처리
                            if (extraCharStat[optionKvp.Key] < 0.001f)
                                extraCharStat[optionKvp.Key] = 0;
                        }
                    }
                }
            }
        }
        SetCharacterStat();
        PrintTotalOption();
    }


    // 부상 상태 회복 시 호출
    public void RestoreOption(Define.Character character)
    {
        foreach (KeyValuePair<Define.Weapon, Dictionary<int, WeaponInfo[]>> kvp in currentWeaponInfo)
        {
            for (int i = 1; i < 5; i++)
            {
                if (kvp.Value[i] == null) continue; // 해당 희귀도의 무기 없음
                for (int j = 0; j < kvp.Value[i].Length; j++)
                {
                    WeaponInfo currentWeaponInfo = kvp.Value[i][j];
                    if (currentWeaponInfo == null) continue;
                    if (currentWeaponInfo.charIndex == (int)character)
                    {
                        if (currentWeaponInfo.weapon.IsChained) continue; // 연계무기 옵션 무시
                        if (currentWeaponInfo.option == null) continue;
                        foreach (KeyValuePair<Define.CharacterStat, float> optionKvp in currentWeaponInfo.option)
                        {
                            extraCharStat[optionKvp.Key] += optionKvp.Value;
                        }
                    }
                }
            }
        }
        SetCharacterStat();
        PrintTotalOption();
    }

    public void PrintTotalOption()
    {
        string tempString = "추가 옵션 >>> ";
        for (int i = 0; i < (int)Define.CharacterStat.end; i++)
        {
            if (extraCharStat[(Define.CharacterStat)i] != 0)
            {
                tempString += $"{(Define.CharacterStat)i} : +{extraCharStat[(Define.CharacterStat)i]} / ";
            }
        }
        Debug.Log(tempString);
    }

    // 가장 무기가 적은 캐릭터에게 무기 추가 => 가능하다면 업그레이드까지
    public void DistributeNewWeapon((Define.Weapon, int, Dictionary<Define.CharacterStat, float>) newWeapon)
    {
        int charIndex = GetMinCountIndex();
        int iconIndex = charWeaponCount[charIndex];
        if (iconIndex >= 2)
        {
            charIndex = -1;
        }
        WeaponInfo newWeaponInfo;
        newWeaponInfo = AddWeapon(newWeapon.Item1, charIndex, newWeapon.Item2, newWeapon.Item3, true, false);
        int i = newWeaponInfo.rarity;

        // 업그레이드 가능한지 확인
        while (i < 4)
        {
            bool temp = CheckIfUpgradeAvailable(newWeaponInfo.weapon.weaponName, i);
            if (temp == false) break;
            i++;
        }
        // StartCoroutine(StageManager.Instance.HighlightIcon(StageManager.Instance.addedIconIndex.Item1, StageManager.Instance.addedIconIndex.Item2));
    }

    public int GetMinCountIndex()
    {
        int minCount = 100;
        int minCountIndex = -1;
        for (int i = 0; i < charWeaponCount.Length; i++)
        {
            if (minCount > charWeaponCount[i])
            {
                minCount = charWeaponCount[i];
                minCountIndex = i;
            }
        }
        return minCountIndex;
    }

    int GetWeaponCount(Define.Weapon weaponName, int rarity)
    {
        int count = 0;
        foreach (WeaponInfo weaponInfo in currentWeaponInfo[weaponName][rarity])
        {
            if (weaponInfo != null) count++;
        }
        return count;
    }

    // 진화 가능 여부 체크 (무기 새로 획득 시 확인) => 가능하면 진화까지
    public bool CheckIfUpgradeAvailable(Define.Weapon weaponName, int rarity)
    {
        List<WeaponInfo> weaponInfos = new List<WeaponInfo>();
        List<WeaponInfo> startWeaponInfos = new List<WeaponInfo>();
        bool isStartWeaponTemp = false;
        int weaponCount = GetWeaponCount(weaponName, rarity);

        // 희귀도 4 미만 무기 3개 획득
        if (weaponCount >= 3 && rarity < 4)
        {
            // 4성 무기 중복 방지
            if (rarity == 3 && hasBlackWeapon)
                return false;

            for (int i = 0; i < currentWeaponInfo[weaponName][rarity].Length; i++)
            {
                if (currentWeaponInfo[weaponName][rarity][i] == null) continue;
                if (currentWeaponInfo[weaponName][rarity][i].isStartWeapon)
                {
                    isStartWeaponTemp = true;
                    startWeaponInfos.Add(currentWeaponInfo[weaponName][rarity][i]);
                }
                else
                {
                    weaponInfos.Add(currentWeaponInfo[weaponName][rarity][i]);
                }
            }
            // 시작무기를 포함하지 않는 무기 셋으로 진화
            if (isStartWeaponTemp == false)
            {
                UpgradeWeapon(weaponInfos, rarity, false);
                return true;
            }
            // 시작무기를 포함하여 진화
            else
            {
                // 1성: 1+2
                if (rarity == 1)
                {
                    // 시작무기가 아닌 무기 1개 => 진화 x
                    if (weaponInfos.Count == 1) return false;

                    List<WeaponInfo> newWeaponInfos = new List<WeaponInfo>();
                    newWeaponInfos.Add(startWeaponInfos[0]);
                    newWeaponInfos.Add(weaponInfos[0]);
                    newWeaponInfos.Add(weaponInfos[1]);
                    UpgradeWeapon(newWeaponInfos, rarity, true);
                    return true;
                }
                // 2성 이상
                else
                {
                    int count = 0;
                    List<WeaponInfo> newWeaponInfos = new List<WeaponInfo>();
                    for (int i = 0; i < startWeaponInfos.Count; i++)
                    {
                        newWeaponInfos.Add(startWeaponInfos[i]);
                        //Debug.Log($"startWeapon: {startWeaponInfos[i].weapon.weaponName}");
                        count++;
                    }
                    for (int i = 0; i < 3 - count; i++)
                    {
                        newWeaponInfos.Add(weaponInfos[i]);
                        //Debug.Log($"weapon: {newWeaponInfos[i].weapon.weaponName}");
                    }
                    UpgradeWeapon(newWeaponInfos, rarity, true);
                    return true;
                }
            }
        }

        return false;
    }

    public void UpgradeWeapon(List<WeaponInfo> weaponInfos, int rarity, bool isStartWeapon)
    {
        Dictionary<Define.CharacterStat, float> totalOption = new Dictionary<Define.CharacterStat, float>();

        for (int i = 0; i < 3; i++)
        {
            DeleteWeapon(weaponInfos[i], true);
        }
        
        totalOption = GetWeaponOption(weaponInfos[0].weaponName, rarity + 1);

        int charIndex;
        int iconIndex;
        if (isStartWeapon)
        {
            charIndex = weaponInfos[0].charIndex;
        }
        else
        {
            charIndex = GetMinCountIndex();
            iconIndex = charWeaponCount[charIndex];
            if (iconIndex >= 2)
            {
                charIndex = -1;
            }
        }

        AddWeapon(weaponInfos[0].weaponName, charIndex, rarity + 1, totalOption, true, isStartWeapon);

        Debug.Log($"{weaponInfos[0].weaponName} 진화! / 희귀도: {rarity + 1}");
        StageManager.Instance.SetWeaponNotice(weaponInfos[0].weaponName, rarity + 1, totalOption);
        //CalculateTotalOption();

        // 재조합 슬롯 비우기
        StageManager.Instance.EmptyRecombinationSlot();

        // 빈칸 발생시 저장칸에서 무기 보충
        SupplementWeapon();
    }

    void SupplementWeapon()
    {
        // 빈칸 발생시 저장칸에서 무기 보충
        for (int i = 0; i < 3; i++)
        {
            if (charWeaponCount[i] < 2 && storageCount > 0)
            {
                WeaponInfo storageWeapon = CheckIfWeaponExists(StageManager.Instance.weaponIconsTrans.GetChild(3).GetChild(0).GetComponent<InventorySlot>().weaponInfo);
                DeleteWeapon(storageWeapon, true);
                AddWeapon(storageWeapon.weapon.weaponName, i, storageWeapon.rarity, storageWeapon.option, true, false);
                break;
            }
        }
    }

    #region recombination
    public WeaponInfo GetNewRandomWeapon(WeaponInfo firstWeaponInfo, WeaponInfo secondWeaponInfo)
    {
        WeaponInfo newWeaponInfo = new WeaponInfo();
        int randInt = (int)firstWeaponInfo.weapon.weaponName;
        while ((int)firstWeaponInfo.weapon.weaponName == randInt || (int)secondWeaponInfo.weapon.weaponName == randInt)
        {
            randInt = Random.Range(0, (int)Define.Weapon.왜검 + 1);
        }
        newWeaponInfo.weaponName = (Define.Weapon)randInt;
        int rand = Random.Range(0, 2);
        
        newWeaponInfo.rarity = firstWeaponInfo.rarity;

        
        // 검정 무기가 있다면 50% 확률로 희귀도++
        if (hasBlackWeapon && firstWeaponInfo.rarity < 3 && rand == 0)
        {
            Debug.Log("성공!");
            newWeaponInfo.rarity = firstWeaponInfo.rarity + 1;
        }

        newWeaponInfo.option = GetWeaponOption(newWeaponInfo.weaponName, newWeaponInfo.rarity);

        return newWeaponInfo;
    }

    public void DoRecombination(WeaponInfo newWeaponInfo, WeaponInfo firstDeletedWeaponInfo, WeaponInfo secondDeletedWeaponInfo)
    {
        DeleteWeapon(firstDeletedWeaponInfo, true);
        DeleteWeapon(secondDeletedWeaponInfo, true);
        DistributeNewWeapon((newWeaponInfo.weaponName, newWeaponInfo.rarity, newWeaponInfo.option));
        Debug.Log($"{newWeaponInfo.weaponName} 희귀도 {newWeaponInfo.rarity} 생성됨!");
        SupplementWeapon();
    }
    #endregion

    public Define.Weapon GetOriginalWeaponName(Define.Weapon weaponName)
    {
        Define.Weapon originalWeaponName = weaponName;

        switch (weaponName)
        {
            case Define.Weapon.별운검:
                originalWeaponName = Define.Weapon.환도;
                break;
            case Define.Weapon.협도:
                originalWeaponName = Define.Weapon.월도;
                break;
            case Define.Weapon.은입사철퇴:
                originalWeaponName = Define.Weapon.철퇴;
                break;
            case Define.Weapon.은입사철퇴연계:
                originalWeaponName = Define.Weapon.철퇴연계;
                break;
            case Define.Weapon.야태도:
                originalWeaponName = Define.Weapon.왜검;
                break;
            case Define.Weapon.당파:
                originalWeaponName = Define.Weapon.창;
                break;
            case Define.Weapon.마상편곤:
                originalWeaponName = Define.Weapon.편곤;
                break;
            case Define.Weapon.마상편곤연계:
                originalWeaponName = Define.Weapon.편곤연계;
                break;
            case Define.Weapon.각궁_불화살:
                originalWeaponName = Define.Weapon.각궁;
                break;
            case Define.Weapon.승자총통_삼연자포:
                originalWeaponName = Define.Weapon.승자총통;
                break;
            case Define.Weapon.현자총통_비격진천뢰:
                originalWeaponName = Define.Weapon.현자총통;
                break;
            case Define.Weapon.천자총통_대장군전:
                originalWeaponName = Define.Weapon.천자총통;
                break;
            case Define.Weapon.철질려:
                originalWeaponName = Define.Weapon.석회가루;
                break;
            case Define.Weapon.석궁_불화살:
                originalWeaponName = Define.Weapon.석궁;
                break;
            case Define.Weapon.변이중화차:
                originalWeaponName = Define.Weapon.화차;
                break;
        }
        return originalWeaponName;
    }

    public Define.Weapon GetUpgradedWeaponName(Define.Weapon weaponName)
    {
        Define.Weapon upgradedWeaponName = Define.Weapon.end;

        switch (weaponName)
        {
            case Define.Weapon.환도:
                upgradedWeaponName = Define.Weapon.별운검;
                break;
            case Define.Weapon.월도:
                upgradedWeaponName = Define.Weapon.협도;
                break;
            case Define.Weapon.철퇴:
                upgradedWeaponName = Define.Weapon.은입사철퇴;
                break;
            case Define.Weapon.철퇴연계:
                upgradedWeaponName = Define.Weapon.은입사철퇴연계;
                break;
            case Define.Weapon.왜검:
                upgradedWeaponName = Define.Weapon.야태도;
                break;
            case Define.Weapon.창:
                upgradedWeaponName = Define.Weapon.당파;
                break;
            case Define.Weapon.편곤:
                upgradedWeaponName = Define.Weapon.마상편곤;
                break;
            case Define.Weapon.편곤연계:
                upgradedWeaponName = Define.Weapon.마상편곤연계;
                break;
            case Define.Weapon.각궁:
                upgradedWeaponName = Define.Weapon.각궁_불화살;
                break;
            case Define.Weapon.승자총통:
                upgradedWeaponName = Define.Weapon.승자총통_삼연자포;
                break;
            case Define.Weapon.현자총통:
                upgradedWeaponName = Define.Weapon.현자총통_비격진천뢰;
                break;
            case Define.Weapon.천자총통:
                upgradedWeaponName = Define.Weapon.천자총통_대장군전;
                break;
            case Define.Weapon.석회가루:
                upgradedWeaponName = Define.Weapon.철질려;
                break;
            case Define.Weapon.석궁:
                upgradedWeaponName = Define.Weapon.석궁_불화살;
                break;
            case Define.Weapon.화차:
                upgradedWeaponName = Define.Weapon.변이중화차;
                break;
        }
        return upgradedWeaponName;
    }

    public Define.Weapon GetChainedWeaponName(Define.Weapon weaponName)
    {
        Define.Weapon chainedWeaponName = Define.Weapon.end;

        switch (weaponName)
        {
            case Define.Weapon.편곤:
                chainedWeaponName = Define.Weapon.편곤연계;
                break;
            case Define.Weapon.철퇴:
                chainedWeaponName = Define.Weapon.철퇴연계;
                break;
            case Define.Weapon.은입사철퇴:
                chainedWeaponName = Define.Weapon.은입사철퇴연계;
                break;
            case Define.Weapon.마상편곤:
                chainedWeaponName = Define.Weapon.마상편곤연계;
                break;
            case Define.Weapon.각궁_불화살:
                chainedWeaponName = Define.Weapon.각궁_불화살연계;
                break;
            case Define.Weapon.석궁_불화살:
                chainedWeaponName = Define.Weapon.석궁_불화살연계;
                break;
            case Define.Weapon.현자총통_비격진천뢰:
                chainedWeaponName = Define.Weapon.현자총통_비격진천뢰연계;
                break;
        }

        return chainedWeaponName;
    }

    // public void UpgradeWeapon(Define.Weapon targetWeapon, int index)
    // {
    //     WeaponUpgradeCount[(int)targetWeapon][index]++;
    // }
    // public (Define.Weapon, int) GetAvailableUpgrades()
    // {
    //     Define.Weapon resultWeapon = Define.Weapon.Cheonja;
    //     List<(Define.Weapon, int)> availableList = new List<(Define.Weapon, int)>();
    //     for (int i = 0; i < WeaponUpgradeValue.Count; i++)
    //     {
    //         for (int j = 0; j < WeaponUpgradeValue[i].Count; j++)
    //         {
    //             if (WeaponUpgradeValue[i][j] > 0)
    //             {
    //                 availableList.Add(((Define.Weapon)i, j));
    //             }
    //         }
    //     }

    //     int index = Random.Range(0, availableList.Count - 1);

    //     return availableList[index];
    // }

}
