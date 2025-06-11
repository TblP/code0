using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwitchMaterial : MonoBehaviour
{

    public Material highlightMaterial;  // Материал, который будет добавляться
    public Image fillAmount;  // Исходный материал объекта
    private GameObject lastHitObject;   // Последний объект, который был пересечен лучом

    public Material Holo;
    public float speed = 10f;
    public float dissolveValue = 0f;
    public float dissolveValueMax = 1f;// Начальное значение Dissolve
    public float dissolveSpeed = 1.0f; // Скорость изменения Dissolve

    public bool addHolo = false; // Флаг для отслеживания состояния

    public bool ReadyToFix = false;

    private SpaceShipSmoke lockOutline;
    public Upgrades upg;
    void Start()
    {
        if (transform.parent.name.Contains("cam"))
        {
            dissolveSpeed = dissolveSpeed / (speed);
        }
        if (transform.parent.name.Contains("Transport"))
        {
            speed -= 100 * upg.acelDestroyTrash;
            dissolveSpeed = dissolveSpeed / (speed);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int layerMask = 1 << LayerMask.NameToLayer("Transport");
        layerMask = ~layerMask;

        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, fwd, out hit, 35, layerMask))
        {
            if (hit.collider != null && hit.collider.transform.parent != null)
            {
                if (hit.collider.transform.parent.name.Contains("broken"))
                {
                    //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 10, Color.white);
                    //Debug.Log(hit.collider.gameObject.name);

                    GameObject hitObject = hit.collider.gameObject;
                    lockOutline = hitObject.GetComponentInParent<SpaceShipSmoke>();
                    // Если луч пересекает новый объект
                    if (lastHitObject != hitObject)
                    {
                        // Удалить материал с последнего пересекаемого объекта, если он существует
                        if (lastHitObject != null)
                        {
                            Renderer lastRenderer = lastHitObject.GetComponent<Renderer>();
                            if (lastRenderer != null)
                            {
                                RemoveHighlightMaterial(lastRenderer);
                            }
                        }

                        // Добавить новый материал на текущий объект
                        Renderer hitRenderer = hitObject.GetComponent<Renderer>();
                        if (hitRenderer != null)
                        {
                            AddHighlightMaterial(hitRenderer);
                        }

                        // Обновить ссылку на последний пересекаемый объект
                        lastHitObject = hitObject;
                    }
                    if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.R))
                    {
                        Renderer hitRenderer = hitObject.GetComponent<Renderer>();
                        OriginalMaterial stat_check = hitObject.GetComponent<OriginalMaterial>();

                        if (!stat_check.state && stat_check.health < 100f)
                        {
                            fillAmount.fillAmount += dissolveSpeed * Time.deltaTime * upg.acelFixRobot;
                            dissolveValue += dissolveSpeed * Time.deltaTime * upg.acelFixRobot;
                            hitRenderer.materials[0].SetFloat("_Dissolve", dissolveValue);

                            if (dissolveValue >= 1)
                            {
                                for (int i = 0; i < hitRenderer.materials.Length; i++)
                                {
                                    if (hitRenderer.materials[i].name != Holo.name + " (Instance)")
                                    {
                                        addHolo = true;
                                    }
                                    else
                                    {
                                        addHolo = false;
                                    }
                                }
                                if (addHolo)
                                {
                                    AddHighlightMaterial2(hitRenderer);
                                    addHolo = false;
                                    fillAmount.fillAmount = 0;
                                    dissolveValue = 0;
                                }
                            }
                        }

                        if (stat_check.state && stat_check.repairReady)
                        {
                            //Debug.Log("repair_coming");
                            fillAmount.fillAmount += dissolveSpeed * Time.deltaTime * upg.acelFixRobot;
                            dissolveValueMax -= dissolveSpeed * Time.deltaTime * upg.acelFixRobot;
                            hitRenderer.materials[0].SetFloat("_Dissolve", dissolveValueMax);

                            if (dissolveValueMax <= 0)
                            {
                                RemoveHighlightMaterial2(hitRenderer);
                                fillAmount.fillAmount = 0;
                                dissolveValueMax = 1;
                                stat_check.repairReady = false;
                                stat_check.final2 = true;
                            }
                        }
                    }
                    else
                    {
                        OriginalMaterial stat_check = hitObject.GetComponent<OriginalMaterial>();
                        Renderer hitRenderer = hitObject.GetComponent<Renderer>();

                        if (!stat_check.state && stat_check.health < 100f)
                        {
                            fillAmount.fillAmount = 0;
                            if (dissolveValue > 0f)
                            {
                                dissolveValue -= Time.deltaTime * 2f;
                            }

                            if (dissolveValue < 0f)
                            {
                                dissolveValue = 0f;
                            }

                            hitRenderer.materials[0].SetFloat("_Dissolve", dissolveValue);
                        }

                        if (stat_check.state && stat_check.repairReady)
                        {
                            fillAmount.fillAmount = 0;
                            if (dissolveValueMax > 0f)
                            {
                                dissolveValueMax += Time.deltaTime * 2f;
                            }

                            if (dissolveValueMax > 1f)
                            {
                                dissolveValueMax = 1f;
                            }
                            hitRenderer.materials[0].SetFloat("_Dissolve", dissolveValueMax);
                        }
                    }
                }
                else
                {
                    // Если луч ни с чем не пересекается, удалить материал с последнего пересекаемого объекта
                    if (lastHitObject != null)
                    {
                        Renderer lastRenderer = lastHitObject.GetComponent<Renderer>();
                        if (lastRenderer != null)
                        {
                            RemoveHighlightMaterial(lastRenderer);
                        }

                        // Сбросить ссылку на последний пересекаемый объект
                        lastHitObject = null;
                    }

                }
            }

            if (lockOutline != null)
            {
                if (lockOutline.goAway)
                {
                    if (lastHitObject != null)
                    {
                        Renderer lastRenderer = lastHitObject.GetComponent<Renderer>();
                        Material[] materials = lastRenderer.materials;
                        if (materials.Length == 2)
                        {
                            materials[1] = null;
                            lastRenderer.materials = materials;
                        }
                    }


                }
            }
        }
            
    }

    private void AddHighlightMaterial2(Renderer renderer)
    {
        Material[] materials = renderer.materials;
        Material[] newMaterials = new Material[materials.Length + 1];
        for (int i = 0; i < materials.Length; i++)
        {
            newMaterials[i] = materials[i];
        }
        newMaterials[materials.Length] = Holo;
        renderer.materials = newMaterials;
    }
    private void RemoveHighlightMaterial2(Renderer renderer)
    {
        Material[] materials = renderer.materials;
        if (materials.Length > 1)
        {
            Material[] newMaterials = new Material[materials.Length - 1];

            if (materials.Length >= 3)
            {
                newMaterials[0] = materials[0];
                newMaterials[1] = materials[2];
            }
            else
            {
                newMaterials[0] = materials[0];
            }
            renderer.materials = newMaterials;
        }
    }

    private void AddHighlightMaterial(Renderer renderer)
    {
        Material[] materials = renderer.materials;
        Material[] newMaterials = new Material[materials.Length + 1];
        for (int i = 0; i < materials.Length; i++)
        {
            newMaterials[i] = materials[i];
        }
        newMaterials[materials.Length] = highlightMaterial;
        renderer.materials = newMaterials;
    }

    private void RemoveHighlightMaterial(Renderer renderer)
    {
        Material[] materials = renderer.materials;
        if (materials.Length > 1)
        {
            Material[] newMaterials = new Material[materials.Length - 1];

            if (materials.Length >= 3)
            {
                newMaterials[0] = materials[0];
                newMaterials[1] = Holo;
            }
            else
            {
                newMaterials[0] = materials[0];
            }
            renderer.materials = newMaterials;
        }
    }
}