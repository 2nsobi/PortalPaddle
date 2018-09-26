using UnityEngine;

public class FilterController : MonoBehaviour
{
    SpriteRenderer currentFilter;
    SpriteRenderer previousFilter;
    bool fade2Filter = false;
    bool clearFilters = false;
    float alpha1 = 1;
    float alpha2 = 0;
    SpriteRenderer[] filters;

    public static FilterController Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        filters = new SpriteRenderer[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            filters[i] = transform.GetChild(i).GetComponent<SpriteRenderer>();
        }
    }

    public void Fade2Filter(string filterName)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (filterName == filters[i].name)
            {
                alpha1 = 1;
                alpha2 = 0;
                currentFilter = filters[i];
                fade2Filter = true;
                break;
            }
        }
    }

    public void ClearFilters()
    {
        clearFilters = true;
    }

    void Update()
    {
        if (fade2Filter)
        {
            if (previousFilter != null)
            {
                alpha1 -= Time.deltaTime * 2;
                previousFilter.color = new Color(1, 1, 1, alpha1);
            }

            alpha2 += Time.deltaTime * 2;
            currentFilter.color = new Color(1, 1, 1, alpha2);

            if (currentFilter.color.a >= 1)
            {
                fade2Filter = false;
                previousFilter = currentFilter;
            }
        }

        if (clearFilters)
        {
            if (previousFilter != null)
            {
                alpha1 -= Time.deltaTime * 2;
                previousFilter.color = new Color(1, 1, 1, alpha1);

                if (previousFilter.color.a <= 0)
                {
                    clearFilters = false;
                    this.gameObject.SetActive(false);
                }
            }
        }
    }

    public void OnDisable()
    {
        for (int i = 0; i < filters.Length; i++)
        {
            filters[i].color = new Color(1, 1, 1, 0);
        }
    }
}
